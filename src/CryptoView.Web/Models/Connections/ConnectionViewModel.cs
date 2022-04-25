using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CryptoView.Web.Models.Connections
{
    public class ConnectionViewModel
    {
        [Required]
        public int ExchangeId { get; set; }

        [Required, MinLength(10)]
        public string ApiKey { get; set; }

        [Required, MinLength(10)]
        public string ApiSecret { get; set; }

        public IEnumerable<SelectListItem> Exchanges { get; set; } = new List<SelectListItem>();
    }
}
