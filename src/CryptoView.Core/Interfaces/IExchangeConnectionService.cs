using CryptoView.Core.Entities;

namespace CryptoView.Core.Interfaces
{
    public interface IExchangeConnectionService
    {
        Task<IEnumerable<ExchangeConnection>> GetConnectionsForUserAsync(string userId);
        Task<ExchangeConnection> CreateConnectionAsync(ExchangeConnection connection);
    }
}