namespace APNAPASHU.DataContract.Entity
{
    public class Message : BaseEntity
    {
        public int ConversationId { get; set; }

        public int SenderUserId { get; set; }

        public string MessageText { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime? ReadDate { get; set; }
    }
}
