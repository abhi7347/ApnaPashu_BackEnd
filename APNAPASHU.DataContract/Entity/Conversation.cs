namespace APNAPASHU.DataContract.Entity
{
    public class Conversation : BaseEntity
    {
        public int? AnimalId { get; set; }

        public int SenderUserId { get; set; }

        public int ReceiverUserId { get; set; }

        public string? LastMessage { get; set; }

        public DateTime? LastMessageDate { get; set; }

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
