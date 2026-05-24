namespace APNAPASHU.DataContract.Models.Chat
{
    public class CreateConversationRequestModel
    {
        public int ReceiverUserId { get; set; }

        public int? AnimalId { get; set; }

        public string? InitialMessage { get; set; }
    }
}
