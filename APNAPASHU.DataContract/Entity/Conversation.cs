namespace APNAPASHU.DataContract.Entity
{
    public class Conversation : BaseEntity
    {
        public int? AnimalId { get; set; }

        public int SenderUserId { get; set; }

        public int ReceiverUserId { get; set; }

        public string? LastMessage { get; set; }

        public DateTime? LastMessageDate { get; set; }
    }
}
