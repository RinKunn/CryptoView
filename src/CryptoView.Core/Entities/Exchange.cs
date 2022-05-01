using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoView.Core.Entities
{
    public class Exchange
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ApiBaseUrl { get; set; }
        public string ImgUrl { get; set; }
    }
}
