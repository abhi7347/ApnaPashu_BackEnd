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
    public class MessageRepository : BaseRepository, IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(IConfiguration configuration, AppDbContext context) : base(configuration)
        {
            _context = context;
        }

        public async Task<Message?> GetMessageByIdAsync(int messageId)
        {
            try
            {
                return await _context.Messages
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == messageId && m.IsDeleted == false);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<MessageResponseModel>> GetConversationMessagesAsync(int conversationId, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@ConversationId", conversationId, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@PageNumber", pageNumber, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@PageSize", pageSize, DbType.Int32, ParameterDirection.Input);

                var result = await GetAsyncList<MessageResponseModel>(
                    "[dbo].[usp_Get_ConversationMessages]",
                    parameter,
                    CommandType.StoredProcedure
                );

                return result ?? new List<MessageResponseModel>();
            }
            catch
            {
                return new List<MessageResponseModel>();
            }
        }

        public async Task<int> CreateMessageAsync(Message message)
        {
            try
            {
                message.CreatedDate = DateTime.UtcNow;
                message.IsActive = true;
                message.IsDeleted = false;
                message.IsRead = false;

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                return message.Id ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> MarkMessageAsReadAsync(int messageId)
        {
            try
            {
                var message = await _context.Messages
                    .FirstOrDefaultAsync(m => m.Id == messageId && m.IsDeleted == false);

                if (message == null)
                    return false;

                message.IsRead = true;
                message.ReadDate = DateTime.UtcNow;

                _context.Messages.Update(message);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarkConversationMessagesAsReadAsync(int conversationId, int userId)
        {
            try
            {
                var messages = await _context.Messages
                    .Where(m => m.ConversationId == conversationId && 
                           m.SenderUserId != userId && 
                           m.IsRead == false && 
                           m.IsDeleted == false)
                    .ToListAsync();

                if (messages.Count == 0)
                    return true;

                foreach (var message in messages)
                {
                    message.IsRead = true;
                    message.ReadDate = DateTime.UtcNow;
                }

                _context.Messages.UpdateRange(messages);
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
