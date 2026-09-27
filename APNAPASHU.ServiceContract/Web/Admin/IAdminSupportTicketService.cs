using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.SupportTicket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APNAPASHU.ServiceContract.Web.Admin
{
    public interface IAdminSupportTicketService
    {
        Task<JsonModel<List<AdminSupportTicketResponseDto>>> GetAllTicketsAsync(AdminSupportTicketFilterDto filter);
        Task<JsonModel<AdminSupportTicketResponseDto>> GetTicketByIdAsync(int ticketId);
        Task<JsonModel<int>> AddReplyAsync(int adminUserId, AdminReplyTicketDto model);
        Task<JsonModel<bool>> UpdateTicketStatusAsync(int updatedByUserId, UpdateTicketStatusDto model);
    }
}
