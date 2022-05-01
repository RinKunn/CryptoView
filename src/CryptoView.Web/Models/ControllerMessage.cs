namespace CryptoView.Web.Models
{
    public class ControllerMessage
    {
        public MessageType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public ControllerMessage(string message, MessageType type, string title = null)
        {
            Message = message;
            Type = type;
            Title = title;
        }

        public enum MessageType
        {
            Info,
            Warn,
            Error,
        }
    }
}
