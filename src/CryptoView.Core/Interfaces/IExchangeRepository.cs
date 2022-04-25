using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoView.Core.Entities;

namespace CryptoView.Core.Interfaces
{
    public interface IExchangeRepository
    {
        Task<Exchange> GetAllAsync();
    }
}
