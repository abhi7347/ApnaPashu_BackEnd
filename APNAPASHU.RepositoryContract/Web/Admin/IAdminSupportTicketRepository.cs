using APNAPASHU.DataContract.Entity.Admin;
using APNAPASHU.DataContract.Models.Web.Admin.SupportTicket;

namespace APNAPASHU.RepositoryContract.Web.Admin
{
    public interface IAdminSupportTicketRepository
    {
        Task<List<AdminSupportTicketResponseDto>> GetAllTicketsAsync(AdminSupportTicketFilterDto filter);
        Task<AdminSupportTicketResponseDto?> GetTicketByIdAsync(int ticketId);
        Task<int> AddReplyAsync(SupportTicketMessage message);
        Task<bool> UpdateStatusAsync(int ticketId, string status, int updatedBy);
    }
}
