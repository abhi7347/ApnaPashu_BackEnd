using System;

namespace APNAPASHU.DataContract.Entity.Admin
{
    public class SupportTicket
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // General, Complaint, Fraud Report, Technical Issue, Payment/Transaction
        public string Priority { get; set; } = "Low"; // Low, Medium, High, Urgent
        public string Status { get; set; } = "Open"; // Open, In Progress, Resolved, Closed
        public string? ImagesJson { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }

    public class SupportTicketMessage
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int SenderUserId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderRole { get; set; } = "User"; // User or Admin
        public string Message { get; set; } = string.Empty;
        public string? ImagesJson { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}

