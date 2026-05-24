namespace APNAPASHU.DataContract.Entity
{
    public class Message : BaseEntity
    {
        public int ConversationId { get; set; }

        public int SenderUserId { get; set; }

        public string MessageText { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime? ReadDate { get; set; }

        // Override base properties to match database schema naming
        public new int CreatedBy { get; set; }

        public new DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public new bool IsDeleted { get; set; }
    }
}
