using System.ComponentModel.DataAnnotations;

namespace CryptoView.Web.Models.ApiManagement
{
    public class ApiInfo
    {
        public string? Id { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Secret { get; set; }

        public bool IsTradingLocked { get; set; }
        public ExchangeInfo? ExchangeInfo { get; set; }
    }
}
