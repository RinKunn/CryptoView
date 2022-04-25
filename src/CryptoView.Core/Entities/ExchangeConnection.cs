using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoView.Core.Entities
{
    public class ExchangeConnection
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public DateTime Created { get; set; }
        public string UserId { get; set; }  
        public string ExchangeId { get; set; }
        public Exchange Exchange { get; set; }
    }
}