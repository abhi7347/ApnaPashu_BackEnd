using APNAPASHU.DataContract.Entity;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Chat;
using APNAPASHU.RepositoryContract.Web;
using APNAPASHU.ServiceContract.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace APNAPASHU.Service.Web
{
    public class MessageService : BaseService, IMessageService
    {
        private readonly IMessageRepository _repository;
        private readonly IConversationRepository _conversationRepository;

        public MessageService(
            IMessageRepository repository,
            IConversationRepository conversationRepository,
            IHttpContextAccessor accessor,
            IConfiguration configuration) : base(accessor, configuration)
        {
            _repository = repository;
            _conversationRepository = conversationRepository;
        }

        public async Task<JsonModel<MessageResponseModel?>> GetMessageByIdAsync(int messageId)
        {
            try
            {
                var message = await _repository.GetMessageByIdAsync(messageId);

                if (message == null)
                    return new JsonModel<MessageResponseModel?>(null, "Message not found", (int)HttpStatusCode.NotFound);

                var response = new MessageResponseModel
                {
                    Id = message.Id ?? 0,
                    ConversationId = message.ConversationId,
                    SenderUserId = message.SenderUserId,
                    MessageText = message.MessageText,
                    IsRead = message.IsRead,
                    ReadDate = message.ReadDate,
                    CreatedDate = message.CreatedDate
                };

                return new JsonModel<MessageResponseModel?>(response, "Message retrieved successfully", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<MessageResponseModel?>(null, $"Error: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<JsonModel<List<MessageResponseModel>>> GetConversationMessagesAsync(int conversationId, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var messages = await _repository.GetConversationMessagesAsync(conversationId, pageNumber, pageSize);
                return new JsonModel<List<MessageResponseModel>>(messages, "Messages retrieved successfully", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<List<MessageResponseModel>>(new List<MessageResponseModel>(), $"Error: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<JsonModel<MessageResponseModel>> SendMessageAsync(SendMessageRequestModel model, int senderUserId)
        {
            try
            {
                if (model.ConversationId == 0)
                    return new JsonModel<MessageResponseModel>(null, "Invalid conversation ID", (int)HttpStatusCode.BadRequest);

                if (string.IsNullOrWhiteSpace(model.MessageText))
                    return new JsonModel<MessageResponseModel>(null, "Message text is required", (int)HttpStatusCode.BadRequest);

                var conversation = await _conversationRepository.GetConversationByIdAsync(model.ConversationId);
                if (conversation == null)
                    return new JsonModel<MessageResponseModel>(null, "Conversation not found", (int)HttpStatusCode.NotFound);

                var message = new Message
                {
                    ConversationId = model.ConversationId,
                    SenderUserId = senderUserId,
                    MessageText = model.MessageText,
                    IsRead = false,
                    CreatedBy = senderUserId
                };

                var messageId = await _repository.CreateMessageAsync(message);

                if (messageId == 0)
                    return new JsonModel<MessageResponseModel>(null, "Failed to send message", (int)HttpStatusCode.InternalServerError);

                // Update conversation last message
                await _conversationRepository.UpdateLastMessageAsync(model.ConversationId, model.MessageText, senderUserId);

                var response = new MessageResponseModel
                {
                    Id = messageId,
                    ConversationId = model.ConversationId,
                    SenderUserId = senderUserId,
                    MessageText = model.MessageText,
                    IsRead = false,
                    CreatedDate = DateTime.UtcNow
                };

                return new JsonModel<MessageResponseModel>(response, "Message sent successfully", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<MessageResponseModel>(null, $"Error: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<JsonModel<object>> MarkMessageAsReadAsync(int messageId)
        {
            try
            {
                if (messageId == 0)
                    return new JsonModel<object>(null, "Invalid message ID", (int)HttpStatusCode.BadRequest);

                var result = await _repository.MarkMessageAsReadAsync(messageId);

                if (!result)
                    return new JsonModel<object>(null, "Failed to mark message as read", (int)HttpStatusCode.InternalServerError);

                return new JsonModel<object>(true, "Message marked as read", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<object>(null, $"Error: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<JsonModel<object>> MarkConversationMessagesAsReadAsync(int conversationId, int userId)
        {
            try
            {
                if (conversationId == 0 || userId == 0)
                    return new JsonModel<object>(null, "Invalid conversation or user ID", (int)HttpStatusCode.BadRequest);

                var result = await _repository.MarkConversationMessagesAsReadAsync(conversationId, userId);

                if (!result)
                    return new JsonModel<object>(null, "Failed to mark messages as read", (int)HttpStatusCode.InternalServerError);

                return new JsonModel<object>(true, "Messages marked as read", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<object>(null, $"Error: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
