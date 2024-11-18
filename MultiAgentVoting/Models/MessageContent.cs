namespace MultiAgentVoting.Models
{
    internal class MessageContent
    {
        public MessageAction Action { get; set; }

        public object Payload { get; set; }

        public MessageContent(MessageAction action, object payload)
        {
            Action = action;
            Payload = payload;
        }
    }
}
