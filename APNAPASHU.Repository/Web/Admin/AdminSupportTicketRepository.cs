using APNAPASHU.DataContract.Entity.Admin;
using APNAPASHU.DataContract.Enums;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.SupportTicket;
using APNAPASHU.RepositoryContract.Web.Admin;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace APNAPASHU.Repository.Web.Admin
{
    public class AdminSupportTicketRepository : BaseRepository, IAdminSupportTicketRepository
    {
        public AdminSupportTicketRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<AdminSupportTicketResponseDto>> GetAllTicketsAsync(AdminSupportTicketFilterDto filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PageNumber", filter?.PageNumber ?? 1, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", filter?.PageSize ?? 10, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Status", string.IsNullOrWhiteSpace(filter?.Status) || filter.Status == "All" ? null : filter.Status, DbType.String, ParameterDirection.Input);
            parameters.Add("@Category", string.IsNullOrWhiteSpace(filter?.Category) || filter.Category == "All" ? null : filter.Category, DbType.String, ParameterDirection.Input);
            parameters.Add("@Priority", string.IsNullOrWhiteSpace(filter?.Priority) || filter.Priority == "All" ? null : filter.Priority, DbType.String, ParameterDirection.Input);
            parameters.Add("@SearchTerm", string.IsNullOrWhiteSpace(filter?.SearchTerm) ? null : filter.SearchTerm.Trim(), DbType.String, ParameterDirection.Input);

            var result = await GetAsyncList<AdminSupportTicketResponseDto>(
                "[dbo].[usp_Get_AllSupportTickets]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            return result ?? new List<AdminSupportTicketResponseDto>();
        }

        public async Task<AdminSupportTicketResponseDto?> GetTicketByIdAsync(int ticketId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TicketId", ticketId, DbType.Int32, ParameterDirection.Input);

            var (tickets, messages) = await QueryMultipleAsync<AdminSupportTicketResponseDto, AdminSupportTicketMessageResponseDto>(
                "[dbo].[usp_Get_SupportTicketById]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            var ticket = tickets?.FirstOrDefault();
            if (ticket != null)
            {
                ticket.Messages = messages?.ToList() ?? new List<AdminSupportTicketMessageResponseDto>();
            }

            return ticket;
        }

        public async Task<int> AddReplyAsync(SupportTicketMessage message)
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
                    senderName = !string.IsNullOrWhiteSpace(name) ? name.Trim() : "Support Agent";
                }
                catch
                {
                    senderName = "Support Agent";
                }
            }

            var parameters = new DynamicParameters();
            parameters.Add("@TicketId", message.TicketId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@SenderUserId", message.SenderUserId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@SenderName", senderName, DbType.String, ParameterDirection.Input);
            parameters.Add("@SenderRole", message.SenderRole ?? "Admin", DbType.String, ParameterDirection.Input);
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
                throw new ApplicationException(result?.Message ?? "Failed to add support ticket reply.");
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
    }
}
