namespace CryptoView.Web.Models.ApiManagement
{
    public class IndexViewModel
    {
        public IEnumerable<ApiInfo> APIs { get; set; }
        public IEnumerable<ExchangeInfo> ExchangesWithNoAPI { get; set; }
    }
}
