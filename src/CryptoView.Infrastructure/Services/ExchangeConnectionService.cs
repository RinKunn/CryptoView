using CryptoView.Core.Entities;
using CryptoView.Core.Interfaces;

namespace CryptoView.Infrastructure.Services
{
    public class MoqExchangeConnectionService : IExchangeConnectionService
    {
        private readonly static List<Exchange> _exchanges = new List<Exchange>()
        {
            new Exchange() { Id = "1", Name = "Binance", ApiUrl = "binance/api" },
            new Exchange() { Id = "2", Name = "Kraken", ApiUrl = "kraken/api" },
        };

        private readonly static List<ExchangeConnection> _connections = new List<ExchangeConnection>()
        {
            new ExchangeConnection()
            {
                Id = "1",
                ExchangeId = "1",
                Exchange = _exchanges[0],
                Key = "binanceapikey",
                Secret = "binanceapisecret",
                UserId = "",
                Created = DateTime.Now,
            },
            new ExchangeConnection()
            {
                Id = "2",
                ExchangeId = "1",
                Exchange = _exchanges[0],
                Key = "krakenapikey",
                Secret = "krakenapisecret",
                UserId = "",
                Created = DateTime.Now,
            }
        };

        public async Task<bool> CheckConnectionAsync(string exchangeConnectionId)
        {
            await Task.Delay(1000);
            return true;
        }

        public async Task<ExchangeConnection> CreateConnectionAsync(ExchangeConnection connection)
        {
            await Task.Delay(100);
            var exch = _exchanges.FirstOrDefault(x => x.Id == connection.ExchangeId);
            if (exch == null) return null;
            connection.Exchange = exch;
            connection.Created = DateTime.Now;
            connection.Id = Guid.NewGuid().ToString();
            _connections.Add(connection);
            return connection;
        }

        public Task DeleteConnectionAsync(string exchangeConnectionId)
        {
            var connection = _connections.FirstOrDefault(c => c.Id == exchangeConnectionId);
            if(connection != null)
                _connections.Remove(connection);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ExchangeConnection>> GetConnectionsForUserAsync(string userId)
        {
            await Task.Delay(100);
            return _connections;
        }

        public Task<bool> IsExistsFor(string userId, string exchangeId)
        {
            return
                Task.FromResult(
                    _connections.FirstOrDefault(c => c.ExchangeId == exchangeId) != null);
        }
    }
}
