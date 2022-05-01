using CryptoView.Core.Entities;
using CryptoView.Core.Interfaces.Repositories;

namespace CryptoView.Infrastructure.Mocks.Repositories
{
    public class MockApiRepository : IApiRepository
    {
        private readonly IExchangeRepository _exchangeRepository;
        private readonly IList<Api> _entities;

        public MockApiRepository(IExchangeRepository exchangeRepository)
        {
            _exchangeRepository = exchangeRepository;
            _entities = GenerateApiEntites();
        }

        public Task<Api> CreateAsync(Api api)
        {
            _entities.Add(api);
            return Task.FromResult(api);
        }

        public Task DeleteAsync(string id)
        {
            var entity = _entities.FirstOrDefault(x => x.Id == id);
            if (entity != null)
                _entities.Remove(entity);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Api>> GetAllForUserAsync(string usedId)
        {
            return Task.FromResult(_entities.Where(e => e.UserId == usedId));
        }

        public Task<Api> GetByIdAsync(string id, bool includeDetails = false)
        {
            var res = _entities.FirstOrDefault(e => e.Id == id);
            if (!includeDetails)
                res.Exchange = null;
            return Task.FromResult(res);
        }

        public Task<Api> GetForExchange(int exchangeId, string userId, bool inclincludeDetailsudeInner = false)
        {
            var res = _entities.FirstOrDefault(e => e.ExchangeId == exchangeId && e.UserId == userId);
            return Task.FromResult(res);
        }

        public Task<Api> UpdateAsync(Api api)
        {
            var res = _entities.FirstOrDefault(e => e.Id == api.Id);
            if (res != null)
            {
                res.Updated = DateTime.UtcNow;
                res.Key = api.Key;
                res.Secret = api.Secret;
            }
            return Task.FromResult(res);
        }

        private List<Api> GenerateApiEntites()
        {
            var list = new List<Api>();
            int apiCount = 2;
            var exchanges = _exchangeRepository.GetAllAsync().Result.ToList();
            for (int i = 0; i < apiCount; i++)
            {
                list.Add(new Api
                {
                    Id = "api" + (i + 1),
                    Key = Guid.NewGuid().ToString(),
                    Secret = Guid.NewGuid().ToString(),
                    IsTradingLocked = i % 2 == 0,
                    UserId = "39515008-7775-4937-a3dd-96129aeeb875",
                    ExchangeId = exchanges[i].Id,
                    Exchange = exchanges[i],
                    Created = DateTime.UtcNow.AddDays(-10 + i),
                });
                list.Add(new Api
                {
                    Id = "api" + (i + 1 + apiCount),
                    Key = Guid.NewGuid().ToString(),
                    Secret = Guid.NewGuid().ToString(),
                    IsTradingLocked = i % 2 == 0,
                    UserId = "user2",
                    ExchangeId = exchanges[i + 1].Id,
                    Exchange = exchanges[i + 1],
                    Created = DateTime.UtcNow.AddDays(-10 + i + apiCount),
                });
            }
            return list;
        }
    }
}
