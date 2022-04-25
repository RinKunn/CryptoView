namespace CryptoView.Web.Models.Connections
{
    public class Connection
    {
        public string Id { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public bool IsTradingLocked { get; set; }
        public DateTime Created { get; set; }
        public string UserId { get; set; }
        public string ExchangeName { get; set; }
        public int ExchangeId { get; set; }
    }
}
