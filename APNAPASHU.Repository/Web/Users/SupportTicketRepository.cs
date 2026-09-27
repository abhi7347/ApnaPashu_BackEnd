using APNAPASHU.DataContract.Entity.Admin;
using APNAPASHU.DataContract.Enums;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Users.SupportTicket;
using APNAPASHU.RepositoryContract.Web.Users;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace APNAPASHU.Repository.Web.Users
{
    public class SupportTicketRepository : BaseRepository, ISupportTicketRepository
    {
        public SupportTicketRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<int> CreateTicketAsync(SupportTicket ticket, string initialMessage)
        {
            var random = new Random();
            string ticketNumber = string.IsNullOrWhiteSpace(ticket.TicketNumber)
                ? $"TK-{DateTime.UtcNow:yyyyMMdd}-{random.Next(1000, 9999)}"
                : ticket.TicketNumber;

            string senderName = "User";
            try
            {
                var userParam = new DynamicParameters();
                userParam.Add("@UserId", ticket.UserId, DbType.Int32, ParameterDirection.Input);
                var name = await GetQuerySingleOrDefaultAsync<string>(
                    "SELECT RTRIM(LTRIM(CONCAT(FirstName, ' ', LastName))) FROM [dbo].[Users] WHERE [UserId] = @UserId",
                    userParam,
                    CommandType.Text,
                    DataBaseNameEnum.APNAPASHU
                );
                if (!string.IsNullOrWhiteSpace(name)) senderName = name.Trim();
            }
            catch
            {
                senderName = "User";
            }

            var parameters = new DynamicParameters();
            parameters.Add("@TicketNumber", ticketNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("@UserId", ticket.UserId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Subject", ticket.Subject ?? string.Empty, DbType.String, ParameterDirection.Input);
            parameters.Add("@Category", ticket.Category ?? "General", DbType.String, ParameterDirection.Input);
            parameters.Add("@Priority", ticket.Priority ?? "Low", DbType.String, ParameterDirection.Input);
            parameters.Add("@InitialMessage", initialMessage ?? string.Empty, DbType.String, ParameterDirection.Input);
            parameters.Add("@ImagesJson", ticket.ImagesJson, DbType.String, ParameterDirection.Input);
            parameters.Add("@SenderName", senderName, DbType.String, ParameterDirection.Input);

            var result = await GetQuerySingleOrDefaultAsync<SqlResponseModel>(
                "[dbo].[usp_Create_SupportTicket]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            if (result == null || result.Id == null || result.Id <= 0)
            {
                throw new ApplicationException(result?.Message ?? "Failed to create support ticket in database.");
            }

            return result.Id.Value;
        }

        public async Task<int> AddMessageAsync(SupportTicketMessage message)
        {
            string senderName = message.SenderName;
            if (string.IsNullOrWhiteSpace(senderName))
            {
                try
                {
                    var userParam = new DynamicParameters();
                    userParam.Add("@UserId", message.SenderUserId, DbType.Int32, ParameterDirection.Input);
                    var name = await GetQuerySingleOrDefaultAsync<string>(
                        "SELECT RTRIM(LTRIM(CONCAT(FirstName, ' ', LastName))) FROM [dbo].[Users] WHERE [UserId] = @UserId",
                        userParam,
                        CommandType.Text,
                        DataBaseNameEnum.APNAPASHU
                    );
                    senderName = !string.IsNullOrWhiteSpace(name) ? name.Trim() : (message.SenderRole == "Admin" ? "Admin" : "User");
                }
                catch
                {
                    senderName = message.SenderRole == "Admin" ? "Admin" : "User";
                }
            }

            var parameters = new DynamicParameters();
            parameters.Add("@TicketId", message.TicketId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@SenderUserId", message.SenderUserId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@SenderName", senderName, DbType.String, ParameterDirection.Input);
            parameters.Add("@SenderRole", message.SenderRole ?? "User", DbType.String, ParameterDirection.Input);
            parameters.Add("@Message", message.Message ?? string.Empty, DbType.String, ParameterDirection.Input);
            parameters.Add("@ImagesJson", message.ImagesJson, DbType.String, ParameterDirection.Input);

            var result = await GetQuerySingleOrDefaultAsync<SqlResponseModel>(
                "[dbo].[usp_Add_SupportTicketMessage]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            if (result == null || result.Id == null || result.Id <= 0)
            {
                throw new ApplicationException(result?.Message ?? "Failed to add support ticket message.");
            }

            return result.Id.Value;
        }

        public async Task<bool> UpdateStatusAsync(int ticketId, string status, int updatedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TicketId", ticketId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Status", status, DbType.String, ParameterDirection.Input);
            parameters.Add("@UpdatedBy", updatedBy, DbType.Int32, ParameterDirection.Input);

            var rows = await GetQuerySingleOrDefaultAsync<int>(
                "[dbo].[usp_Update_SupportTicketStatus]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            return rows > 0;
        }

        public async Task<SupportTicketResponseDto?> GetTicketByIdAsync(int ticketId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TicketId", ticketId, DbType.Int32, ParameterDirection.Input);

            var (tickets, messages) = await QueryMultipleAsync<SupportTicketResponseDto, SupportTicketMessageResponseDto>(
                "[dbo].[usp_Get_SupportTicketById]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            var ticket = tickets?.FirstOrDefault();
            if (ticket != null)
            {
                ticket.Messages = messages?.ToList() ?? new List<SupportTicketMessageResponseDto>();
            }

            return ticket;
        }

        public async Task<List<SupportTicketResponseDto>> GetUserTicketsAsync(int userId, SupportTicketFilterDto filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageNumber", filter?.PageNumber ?? 1, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", filter?.PageSize ?? 10, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Status", string.IsNullOrWhiteSpace(filter?.Status) || filter.Status == "All" ? null : filter.Status, DbType.String, ParameterDirection.Input);
            parameters.Add("@Category", string.IsNullOrWhiteSpace(filter?.Category) || filter.Category == "All" ? null : filter.Category, DbType.String, ParameterDirection.Input);
            parameters.Add("@SearchTerm", string.IsNullOrWhiteSpace(filter?.SearchTerm) ? null : filter.SearchTerm.Trim(), DbType.String, ParameterDirection.Input);

            var result = await GetAsyncList<SupportTicketResponseDto>(
                "[dbo].[usp_Get_UserSupportTickets]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            return result ?? new List<SupportTicketResponseDto>();
        }
    }
}
