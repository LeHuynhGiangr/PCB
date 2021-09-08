namespace EchoBot1.Models
{
    public class ConversationLog
    {
        //The time-stamp of most recent incoming message
        public string Timestamp { get; set; }
        public string ChannelId { get; set; }
        public bool DidAskTheUserName { get; set; } = false;
    }
}
