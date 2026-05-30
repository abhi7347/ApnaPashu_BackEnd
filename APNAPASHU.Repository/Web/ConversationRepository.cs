using APNAPASHU.DataContract.Entity;
using APNAPASHU.DataContract.Models.Chat;
using APNAPASHU.Repository.Data;
using APNAPASHU.RepositoryContract.Web;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace APNAPASHU.Repository.Web
{
    public class ConversationRepository : BaseRepository, IConversationRepository
    {
        private readonly AppDbContext _context;

        public ConversationRepository(IConfiguration configuration, AppDbContext context) : base(configuration)
        {
            _context = context;
        }

        public async Task<Conversation?> GetConversationByIdAsync(int conversationId)
        {
            try
            {
                return await _context.Conversations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == conversationId && c.IsDeleted == false);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<ConversationResponseModel>> GetUserConversationsAsync(int userId, int pageNumber = 1, int pageSize = 20, string? statusFilter = null)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@UserId", userId, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@PageNumber", pageNumber, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@PageSize", pageSize, DbType.Int32, ParameterDirection.Input);
                if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
                {
                    parameter.Add("@StatusFilter", statusFilter, DbType.String, ParameterDirection.Input);
                }
                else
                {
                    parameter.Add("@StatusFilter", null, DbType.String, ParameterDirection.Input);
                }

                var result = await GetAsyncList<ConversationResponseModel>(
                    "[dbo].[usp_Get_UserConversations]",
                    parameter,
                    CommandType.StoredProcedure
                );

                return result ?? new List<ConversationResponseModel>();
            }
            catch
            {
                return new List<ConversationResponseModel>();
            }
        }

        public async Task<Conversation?> GetOrCreateConversationAsync(int senderUserId, int receiverUserId, int? animalId = null)
        {
            var existingConversation = await _context.Conversations
                .FirstOrDefaultAsync(c =>
                    ((c.SenderUserId == senderUserId && c.ReceiverUserId == receiverUserId) ||
                        (c.SenderUserId == receiverUserId && c.ReceiverUserId == senderUserId)) &&
                    c.IsDeleted == false);

            if (existingConversation != null)
                return existingConversation;

            var newConversation = new Conversation
            {
                AnimalId = animalId,
                SenderUserId = senderUserId,
                ReceiverUserId = receiverUserId,
                CreatedBy = senderUserId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            _context.Conversations.Add(newConversation);
            await _context.SaveChangesAsync();

            return newConversation;            
        }

        public async Task<int> CreateConversationAsync(Conversation conversation)
        {
            try
            {
                conversation.CreatedDate = DateTime.UtcNow;
                conversation.IsDeleted = false;

                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();

                return conversation.Id ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> UpdateConversationAsync(Conversation conversation)
        {
            try
            {
                conversation.UpdatedDate = DateTime.UtcNow;
                _context.Conversations.Update(conversation);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateLastMessageAsync(int conversationId, string message, int senderUserId)
        {
            try
            {
                var conversation = await _context.Conversations
                    .FirstOrDefaultAsync(c => c.Id == conversationId && c.IsDeleted == false);

                if (conversation == null)
                    return false;

                conversation.LastMessage = message;
                conversation.LastMessageDate = DateTime.UtcNow;
                conversation.UpdatedBy = senderUserId;
                conversation.UpdatedDate = DateTime.UtcNow;

                _context.Conversations.Update(conversation);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
