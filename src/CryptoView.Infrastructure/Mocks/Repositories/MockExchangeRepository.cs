using CryptoView.Core.Entities;
using CryptoView.Core.Interfaces.Repositories;

namespace CryptoView.Infrastructure.Mocks.Repositories
{
    public class MockExchangeRepository : IExchangeRepository
    {
        private readonly static List<Exchange> _entites = new List<Exchange>()
        {
            new Exchange { Id = 1, Name = "Binance" },
            new Exchange { Id = 2, Name = "Coinbase" },
            new Exchange { Id = 3, Name = "Kraken" },
            new Exchange { Id = 4, Name = "Rucoin" },
        };

        public Task<IEnumerable<Exchange>> GetAllAsync()
        {
            return Task.FromResult((IEnumerable<Exchange>)_entites);
        }

        public Task<Exchange> GetById(int id)
        {
            return Task.FromResult(_entites.FirstOrDefault(e => e.Id == id));
        }
    }
}
