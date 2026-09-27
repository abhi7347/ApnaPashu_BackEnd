using APNAPASHU.DataContract.Entity.Admin;
using APNAPASHU.DataContract.Models.Web.Users.SupportTicket;

namespace APNAPASHU.RepositoryContract.Web.Users
{
    public interface ISupportTicketRepository
    {
        Task<int> CreateTicketAsync(SupportTicket ticket, string initialMessage);
        Task<int> AddMessageAsync(SupportTicketMessage message);
        Task<bool> UpdateStatusAsync(int ticketId, string status, int updatedBy);
        Task<SupportTicketResponseDto?> GetTicketByIdAsync(int ticketId);
        Task<List<SupportTicketResponseDto>> GetUserTicketsAsync(int userId, SupportTicketFilterDto filter);
    }
}

