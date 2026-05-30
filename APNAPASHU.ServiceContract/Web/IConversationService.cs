using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Chat;

namespace APNAPASHU.ServiceContract.Web
{
    public interface IConversationService
    {
        Task<JsonModel<ConversationResponseModel?>> GetConversationByIdAsync(int conversationId);

        Task<JsonModel<List<ConversationResponseModel>>> GetUserConversationsAsync(int userId, int pageNumber = 1, int pageSize = 20, string? statusFilter = null);

        Task<JsonModel<ConversationResponseModel>> CreateConversationAsync(CreateConversationRequestModel model, int currentUserId);

        Task<JsonModel<object>> UpdateLastMessageAsync(int conversationId, string message, int senderUserId);
    }
}
