namespace APNAPASHU.DataContract.Models.Chat
{
    public class MessageResponseModel
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public int SenderUserId { get; set; }

        public string MessageText { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime? ReadDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? SenderName { get; set; }
    }
}
