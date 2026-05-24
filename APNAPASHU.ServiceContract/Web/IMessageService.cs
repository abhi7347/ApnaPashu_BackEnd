using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Chat;

namespace APNAPASHU.ServiceContract.Web
{
    public interface IMessageService
    {
        Task<JsonModel<MessageResponseModel?>> GetMessageByIdAsync(int messageId);

        Task<JsonModel<List<MessageResponseModel>>> GetConversationMessagesAsync(int conversationId, int pageNumber = 1, int pageSize = 50);

        Task<JsonModel<MessageResponseModel>> SendMessageAsync(SendMessageRequestModel model, int senderUserId);

        Task<JsonModel<object>> MarkMessageAsReadAsync(int messageId);

        Task<JsonModel<object>> MarkConversationMessagesAsReadAsync(int conversationId, int userId);
    }
}
