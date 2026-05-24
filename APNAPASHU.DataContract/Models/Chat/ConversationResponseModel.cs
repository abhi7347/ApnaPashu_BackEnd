namespace APNAPASHU.DataContract.Models.Chat
{
    public class ConversationResponseModel
    {
        public int Id { get; set; }

        public int? AnimalId { get; set; }

        public int SenderUserId { get; set; }

        public int ReceiverUserId { get; set; }

        public string? LastMessage { get; set; }

        public DateTime? LastMessageDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? SenderName { get; set; }

        public string? ReceiverName { get; set; }

        public string? AnimalName { get; set; }
    }
}
