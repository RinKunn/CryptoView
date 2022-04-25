namespace CryptoView.Web.Models
{
    public class AccountViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public ApiViewModel Api { get; set; }

        public class ApiViewModel
        {
            public string SourceName { get; set; }
            public string ApiKey { get; set; }
            public string ApiSecret { get; set; }
            public bool IsTradingLocked { get; set; }
        }
    }
}
