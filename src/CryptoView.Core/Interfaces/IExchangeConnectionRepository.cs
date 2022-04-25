using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using CryptoView.Core.Entities;

namespace CryptoView.Core.Interfaces
{
    public interface IExchangeConnectionRepository
    {
        Task<ExchangeConnection> GetAllForUserAsync(string usedId);

    }
}
