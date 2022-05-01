namespace CryptoView.Core.Entities
{
    public class Api : IAggrregateRoot
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string UserId { get; set; }
        public int ExchangeId { get; set; }
        public bool IsTradingLocked { get; set; }
        public Exchange Exchange { get; set; }
    }
}