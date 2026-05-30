using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APNAPASHU.ServiceContract.Web;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Chat;
using Microsoft.AspNetCore.SignalR;
using APNAPASHU.API.Hubs;

namespace APNAPASHU.API.Controllers.Web
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationController : BaseController
    {
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;
        private readonly ILogger<ConversationController> _logger;
        private readonly IHubContext<ChatHub> _hubContext;

        public ConversationController(
            IConversationService conversationService,
            IMessageService messageService,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration,
            ILogger<ConversationController> logger,
            IHubContext<ChatHub> hubContext)
            : base(contextAccessor, configuration)
        {
            _conversationService = conversationService;
            _messageService = messageService;
            _logger = logger;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Get all conversations for the current user
        /// </summary>
        [HttpGet("my-conversations")]
        [ProducesResponseType(typeof(JsonModel<List<ConversationResponseModel>>), 200)]
        public async Task<IActionResult> GetMyConversations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] string? statusFilter = null)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId == 0)
                    return Unauthorized(new JsonModel<object>(null, "User not authenticated", 401));

                var result = await _conversationService.GetUserConversationsAsync(userId, pageNumber, pageSize, statusFilter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving conversations: {ex.Message}");
                return StatusCode(500, new JsonModel<object>(null, "Internal server error", 500));
            }
        }

        /// <summary>
        /// Get a specific conversation
        /// </summary>
        [HttpGet("{conversationId}")]
        [ProducesResponseType(typeof(JsonModel<ConversationResponseModel>), 200)]
        public async Task<IActionResult> GetConversation(int conversationId)
        {
            try
            {
                var result = await _conversationService.GetConversationByIdAsync(conversationId);
                return result.StatusCode == 200 ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving conversation: {ex.Message}");
                return StatusCode(500, new JsonModel<object>(null, "Internal server error", 500));
            }
        }

        /// <summary>
        /// Create a new conversation
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(typeof(JsonModel<ConversationResponseModel>), 200)]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequestModel model)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId == 0)
                    return Unauthorized(new JsonModel<object>(null, "User not authenticated", 401));

                var result = await _conversationService.CreateConversationAsync(model, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating conversation: {ex.Message}");
                return StatusCode(500, new JsonModel<object>(null, "Internal server error", 500));
            }
        }

        /// <summary>
        /// Get messages for a conversation
        /// </summary>
        [HttpGet("{conversationId}/messages")]
        [ProducesResponseType(typeof(JsonModel<List<MessageResponseModel>>), 200)]
        public async Task<IActionResult> GetMessages(int conversationId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var result = await _messageService.GetConversationMessagesAsync(conversationId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving messages: {ex.Message}");
                return StatusCode(500, new JsonModel<object>(null, "Internal server error", 500));
            }
        }

        /// <summary>
        /// Send a message (alternative to SignalR)
        /// </summary>
        [HttpPost("send-message")]
        [ProducesResponseType(typeof(JsonModel<MessageResponseModel>), 200)]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequestModel model)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId == 0)
                    return Unauthorized(new JsonModel<object>(null, "User not authenticated", 401));

                var result = await _messageService.SendMessageAsync(model, userId);
                
                if (result.StatusCode == 200 && result.Data != null)
                {
                    // Broadcast message to all users in the conversation
                    await _hubContext.Clients.Group($"conversation_{model.ConversationId}")
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
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message: {ex.Message}");
                return StatusCode(500, new JsonModel<object>(null, "Internal server error", 500));
            }
        }

        /// <summary>
        /// Mark a message as read
        /// </summary>
        [HttpPost("mark-message-read/{messageId}")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> MarkMessageAsRead(int messageId)
        {
            try
            {
                var result = await _messageService.MarkMessageAsReadAsync(messageId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking message as read: {ex.Message}");
                return StatusCode(500, new JsonModel<object>(null, "Internal server error", 500));
            }
        }

        /// <summary>
        /// Mark all messages in a conversation as read
        /// </summary>
        [HttpPost("{conversationId}/mark-read")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> MarkConversationAsRead(int conversationId)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId == 0)
                    return Unauthorized(new JsonModel<object>(null, "User not authenticated", 401));

                var result = await _messageService.MarkConversationMessagesAsReadAsync(conversationId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking conversation as read: {ex.Message}");
                return StatusCode(500, new JsonModel<object>(null, "Internal server error", 500));
            }
        }
    }
}
