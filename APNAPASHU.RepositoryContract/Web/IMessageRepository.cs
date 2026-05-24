using APNAPASHU.DataContract.Entity;
using APNAPASHU.DataContract.Models.Chat;

namespace APNAPASHU.RepositoryContract.Web
{
    public interface IMessageRepository
    {
        Task<Message?> GetMessageByIdAsync(int messageId);

        Task<List<MessageResponseModel>> GetConversationMessagesAsync(int conversationId, int pageNumber = 1, int pageSize = 50);

        Task<int> CreateMessageAsync(Message message);

        Task<bool> MarkMessageAsReadAsync(int messageId);

        Task<bool> MarkConversationMessagesAsReadAsync(int conversationId, int userId);
    }
}
