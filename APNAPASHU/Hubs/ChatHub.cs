using Microsoft.AspNetCore.SignalR;
using APNAPASHU.ServiceContract.Web;
using APNAPASHU.DataContract.Models.Chat;
using System.Security.Claims;

namespace APNAPASHU.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;
        private readonly ILogger<ChatHub> _logger;

        // Store active connections: conversationId -> list of connectionIds
        private static Dictionary<int, HashSet<string>> ConversationConnections = new();

        public ChatHub(
            IConversationService conversationService,
            IMessageService messageService,
            ILogger<ChatHub> logger)
        {
            _conversationService = conversationService;
            _messageService = messageService;
            _logger = logger;
        }

        /// <summary>
        /// Called when a user joins a conversation
        /// </summary>
        public async Task JoinConversation(int conversationId)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId == 0)
                {
                    await Clients.Caller.SendAsync("ReceiveError", "Unauthorized access");
                    return;
                }

                // Add this connection to the conversation group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");

                // Track the connection
                if (!ConversationConnections.ContainsKey(conversationId))
                    ConversationConnections[conversationId] = new HashSet<string>();

                ConversationConnections[conversationId].Add(Context.ConnectionId);

                _logger.LogInformation($"User {userId} joined conversation {conversationId}");

                // Notify others in the conversation
                await Clients.Group($"conversation_{conversationId}")
                    .SendAsync("UserJoined", new { userId = userId, connectionId = Context.ConnectionId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error joining conversation: {ex.Message}");
                await Clients.Caller.SendAsync("ReceiveError", "Failed to join conversation");
            }
        }

        /// <summary>
        /// Called when a user leaves a conversation
        /// </summary>
        public async Task LeaveConversation(int conversationId)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                
                // Remove from group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");

                // Remove from tracking
                if (ConversationConnections.ContainsKey(conversationId))
                {
                    ConversationConnections[conversationId].Remove(Context.ConnectionId);
                    if (ConversationConnections[conversationId].Count == 0)
                        ConversationConnections.Remove(conversationId);
                }

                _logger.LogInformation($"User {userId} left conversation {conversationId}");

                // Notify others
                await Clients.Group($"conversation_{conversationId}")
                    .SendAsync("UserLeft", new { userId = userId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error leaving conversation: {ex.Message}");
            }
        }

        /// <summary>
        /// Send a message to a conversation
        /// </summary>
        public async Task SendMessage(int conversationId, string messageText)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId == 0)
                {
                    await Clients.Caller.SendAsync("ReceiveError", "Unauthorized access");
                    return;
                }

                if (string.IsNullOrWhiteSpace(messageText))
                {
                    await Clients.Caller.SendAsync("ReceiveError", "Message cannot be empty");
                    return;
                }

                // Save message to database
                var requestModel = new SendMessageRequestModel
                {
                    ConversationId = conversationId,
                    MessageText = messageText.Trim()
                };

                var result = await _messageService.SendMessageAsync(requestModel, userId);

                if (result.StatusCode != 200 || result.Data == null)
                {
                    await Clients.Caller.SendAsync("ReceiveError", result.Message);
                    return;
                }

                // Broadcast message to all users in the conversation
                await Clients.Group($"conversation_{conversationId}")
                    .SendAsync("ReceiveMessage", new
                    {
                        id = result.Data.Id,
                        conversationId = result.Data.ConversationId,
                        senderUserId = result.Data.SenderUserId,
                        senderName = result.Data.SenderName,
                        messageText = result.Data.MessageText,
                        createdDate = result.Data.CreatedDate,
                        isRead = result.Data.IsRead
                    });

                _logger.LogInformation($"Message sent in conversation {conversationId} by user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message: {ex.Message}");
                await Clients.Caller.SendAsync("ReceiveError", "Failed to send message");
            }
        }

        /// <summary>
        /// Mark a message as read
        /// </summary>
        public async Task MarkMessageAsRead(int messageId)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId == 0) return;

                var result = await _messageService.MarkMessageAsReadAsync(messageId);
                
                if (result.StatusCode == 200)
                {
                    await Clients.Caller.SendAsync("MessageMarkedAsRead", new { messageId = messageId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking message as read: {ex.Message}");
            }
        }

        /// <summary>
        /// Mark all messages in a conversation as read
        /// </summary>
        public async Task MarkConversationAsRead(int conversationId)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId == 0) return;

                var result = await _messageService.MarkConversationMessagesAsReadAsync(conversationId, userId);
                
                if (result.StatusCode == 200)
                {
                    await Clients.Group($"conversation_{conversationId}")
                        .SendAsync("ConversationMarkedAsRead", new { conversationId = conversationId, userId = userId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking conversation as read: {ex.Message}");
            }
        }

        /// <summary>
        /// Get list of active users in a conversation (for online status)
        /// </summary>
        public async Task GetActiveUsers(int conversationId)
        {
            try
            {
                var activeCount = ConversationConnections.ContainsKey(conversationId) 
                    ? ConversationConnections[conversationId].Count 
                    : 0;

                await Clients.Caller.SendAsync("ActiveUsersCount", new { conversationId = conversationId, count = activeCount });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting active users: {ex.Message}");
            }
        }

        /// <summary>
        /// Override disconnect to clean up connections
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                // Remove this connection from all conversation groups
                var conversationsToRemove = ConversationConnections
                    .Where(kvp => kvp.Value.Contains(Context.ConnectionId))
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var conversationId in conversationsToRemove)
                {
                    ConversationConnections[conversationId].Remove(Context.ConnectionId);
                    if (ConversationConnections[conversationId].Count == 0)
                        ConversationConnections.Remove(conversationId);
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on disconnect: {ex.Message}");
            }
        }

        /// <summary>
        /// Get authenticated user ID from claims
        /// </summary>
        private int GetAuthenticatedUserId()
        {
            try
            {
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier) ?? Context.User?.FindFirst("UserId");
                if (int.TryParse(userIdClaim?.Value, out int userId))
                    return userId;
                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}
