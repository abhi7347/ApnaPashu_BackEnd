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
    public class ConversationService : BaseService, IConversationService
    {
        private readonly IConversationRepository _repository;

        public ConversationService(
            IConversationRepository repository,
            IHttpContextAccessor accessor,
            IConfiguration configuration) : base(accessor, configuration)
        {
            _repository = repository;
        }

        public async Task<JsonModel<ConversationResponseModel?>> GetConversationByIdAsync(int conversationId)
        {
            try
            {
                var conversation = await _repository.GetConversationByIdAsync(conversationId);

                if (conversation == null)
                    return new JsonModel<ConversationResponseModel?>(null, "Conversation not found", (int)HttpStatusCode.NotFound);

                var response = new ConversationResponseModel
                {
                    Id = conversation.Id ?? 0,
                    AnimalId = conversation.AnimalId,
                    SenderUserId = conversation.SenderUserId,
                    ReceiverUserId = conversation.ReceiverUserId,
                    LastMessage = conversation.LastMessage,
                    LastMessageDate = conversation.LastMessageDate,
                    CreatedDate = conversation.CreatedDate
                };

                return new JsonModel<ConversationResponseModel?>(response, "Conversation retrieved successfully", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<ConversationResponseModel?>(null, $"Error: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<JsonModel<List<ConversationResponseModel>>> GetUserConversationsAsync(int userId, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var conversations = await _repository.GetUserConversationsAsync(userId, pageNumber, pageSize);
                return new JsonModel<List<ConversationResponseModel>>(conversations, "Conversations retrieved successfully", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<List<ConversationResponseModel>>(new List<ConversationResponseModel>(), $"Error: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<JsonModel<ConversationResponseModel>> CreateConversationAsync(CreateConversationRequestModel model, int currentUserId)
        {
            try
            {
                if (model.ReceiverUserId == 0 || currentUserId == 0)
                    return new JsonModel<ConversationResponseModel>(null, "Invalid user IDs", (int)HttpStatusCode.BadRequest);

                if (currentUserId == model.ReceiverUserId)
                    return new JsonModel<ConversationResponseModel>(null, "Cannot start conversation with yourself", (int)HttpStatusCode.BadRequest);

                var conversation = await _repository.GetOrCreateConversationAsync(currentUserId, model.ReceiverUserId, model.AnimalId);

                if (conversation == null)
                    return new JsonModel<ConversationResponseModel>(null, "Failed to create conversation", (int)HttpStatusCode.InternalServerError);

                var response = new ConversationResponseModel
                {
                    Id = conversation.Id ?? 0,
                    AnimalId = conversation.AnimalId,
                    SenderUserId = conversation.SenderUserId,
                    ReceiverUserId = conversation.ReceiverUserId,
                    LastMessage = conversation.LastMessage,
                    LastMessageDate = conversation.LastMessageDate,
                    CreatedDate = conversation.CreatedDate
                };

                return new JsonModel<ConversationResponseModel>(response, "Conversation created successfully", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<ConversationResponseModel>(null, $"Error: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<JsonModel<object>> UpdateLastMessageAsync(int conversationId, string message, int senderUserId)
        {
            try
            {
                if (conversationId == 0)
                    return new JsonModel<object>(null, "Invalid conversation ID", (int)HttpStatusCode.BadRequest);

                var result = await _repository.UpdateLastMessageAsync(conversationId, message, senderUserId);

                if (!result)
                    return new JsonModel<object>(null, "Failed to update conversation", (int)HttpStatusCode.InternalServerError);

                return new JsonModel<object>(true, "Last message updated successfully", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel<object>(null, $"Error: {ex.Message}", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
