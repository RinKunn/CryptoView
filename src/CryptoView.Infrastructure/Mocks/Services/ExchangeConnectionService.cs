using CryptoView.Core.Interfaces;

namespace CryptoView.Infrastructure.Mocks.Services
{
    public class MockExchangeConnectionService : IExchangeConnectionService
    {
        public Task<bool> CheckApiConnection(string apiId)
        {
            return Task.FromResult(true);
        }
    }
}
