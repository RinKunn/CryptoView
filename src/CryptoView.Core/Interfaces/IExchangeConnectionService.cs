using CryptoView.Core.Entities;

namespace CryptoView.Core.Interfaces
{
    public interface IExchangeConnectionService
    {
        Task<bool> CheckApiConnection(string apiId);
    }
}