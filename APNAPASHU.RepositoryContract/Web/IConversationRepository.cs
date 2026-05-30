using APNAPASHU.DataContract.Entity;
using APNAPASHU.DataContract.Models.Chat;

namespace APNAPASHU.RepositoryContract.Web
{
    public interface IConversationRepository
    {
        Task<Conversation?> GetConversationByIdAsync(int conversationId);

        Task<List<ConversationResponseModel>> GetUserConversationsAsync(int userId, int pageNumber = 1, int pageSize = 20, string? statusFilter = null);

        Task<Conversation?> GetOrCreateConversationAsync(int senderUserId, int receiverUserId, int? animalId = null);

        Task<int> CreateConversationAsync(Conversation conversation);

        Task<bool> UpdateConversationAsync(Conversation conversation);

        Task<bool> UpdateLastMessageAsync(int conversationId, string message, int senderUserId);
    }
}
