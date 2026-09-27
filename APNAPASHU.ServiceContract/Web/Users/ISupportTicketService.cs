using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Users.SupportTicket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APNAPASHU.ServiceContract.Web.Users
{
    public interface ISupportTicketService
    {
        Task<JsonModel<int>> CreateTicketAsync(int userId, CreateSupportTicketDto model);
        Task<JsonModel<int>> AddMessageAsync(int senderUserId, string senderRole, AddTicketMessageDto model);
        Task<JsonModel<bool>> UpdateTicketStatusAsync(int updatedByUserId, UpdateTicketStatusDto model);
        Task<JsonModel<SupportTicketResponseDto>> GetTicketByIdAsync(int ticketId);
        Task<JsonModel<List<SupportTicketResponseDto>>> GetUserTicketsAsync(int userId, SupportTicketFilterDto filter);
    }
}

