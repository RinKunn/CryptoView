using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoView.Core.Entities
{
    public class TradeHistoryItem : IAggrregateRoot
    {
        public string Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
    }
}
