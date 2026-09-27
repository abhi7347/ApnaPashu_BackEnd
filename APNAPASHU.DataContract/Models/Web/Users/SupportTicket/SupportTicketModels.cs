using Microsoft.AspNetCore.Http;

namespace APNAPASHU.DataContract.Models.Web.Users.SupportTicket
{
    public class CreateSupportTicketDto
    {
        public string Subject { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public string Description { get; set; } = string.Empty;
        public List<IFormFile>? Attachments { get; set; }
    }

    public class AddTicketMessageDto
    {
        public int TicketId { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<IFormFile>? Attachments { get; set; }
    }

    public class UpdateTicketStatusDto
    {
        public int TicketId { get; set; }
        public string Status { get; set; } = string.Empty; // Open, In Progress, Resolved, Closed
    }

    public class SupportTicketFilterDto : FilterDto
    {
        public string? Status { get; set; }
        public string? Category { get; set; }
        public string? Priority { get; set; }
        public int? UserId { get; set; }
    }

    public class SupportTicketResponseDto
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ImagesJson { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int TotalRecords { get; set; }
        public List<SupportTicketMessageResponseDto> Messages { get; set; } = new();
    }

    public class SupportTicketMessageResponseDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int SenderUserId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderRole { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ImagesJson { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public DateTime CreatedDate { get; set; }
    }
}
