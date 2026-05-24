namespace APNAPASHU.DataContract.Models.Chat
{
    public class SendMessageRequestModel
    {
        public int ConversationId { get; set; }

        public string MessageText { get; set; } = string.Empty;
    }
}
