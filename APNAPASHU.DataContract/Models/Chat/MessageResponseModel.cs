using APNAPASHU.DataContract.Entity;

namespace APNAPASHU.DataContract.Models.Chat
{
    public class MessageResponseModel: BaseEntity
    {
        public int ConversationId { get; set; }

        public int SenderUserId { get; set; }

        public string MessageText { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime? ReadDate { get; set; }
        public string? SenderName { get; set; }
    }
}
