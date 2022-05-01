using CryptoView.Web.Interfaces;
using CryptoView.Web.Models.ApiManagement;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CryptoView.Web.Services
{
    public class MockApiService : IApiService
    {
        public async Task<ApiInfo> GetApiById(string id, string userId)
        {
            await Task.Delay(10);
            return _data.FirstOrDefault(d => d.Id == id);
        }

        public async Task<IEnumerable<ApiInfo>> GetApiList(string userId)
        {
            await Task.Delay(100);
            return _data;
        }

        public async Task<IEnumerable<ExchangeInfo>> ExchangesWithNoAPI(string userId)
        {
            await Task.Delay(100);
            var exists = _data.Select(d => d.ExchangeInfo.Id).Distinct().ToList();
            return _ex.Where(x => !exists.Contains(x.Id));
        }

        public async Task<ExchangeInfo> GetExchangeById(int id)
        {
            await Task.Delay(100);
            return _ex.FirstOrDefault(x => x.Id == id);
        }

        public async Task<ApiInfo> GetApiForExchange(int exchangeId, string userId)
        {
            await Task.Delay(10);
            return _data.FirstOrDefault(d => d.ExchangeInfo.Id == exchangeId);
        }

        public Task CreateApi(ApiInfo apiInfo, string userId)
        {
            apiInfo.Id = Guid.NewGuid().ToString();
            _data.Add(apiInfo);
            return Task.CompletedTask;
        }

        public Task UpdateApi(ApiInfo apiInfo, string userId)
        {
            var entity = _data.FirstOrDefault(d => d.Id == apiInfo.Id);
            if (entity != null)
            {
                entity.Key = apiInfo.Key;
                entity.Secret = apiInfo.Secret;
            }
            return Task.CompletedTask;
        }

        public Task DeleteApi(string apiId, string userId)
        {
            var entity = _data.FirstOrDefault(d => d.Id == apiId);
            if(entity != null) 
                _data.Remove(entity);
            return Task.CompletedTask;
        }


        private readonly static List<ApiInfo> _data = new List<ApiInfo>()
        {
            new ApiInfo()
                {
                    Id = "78781278-u12yy2-21yu21u721",
                    Key = "API Key1_123457890",
                    Secret = "API Secret1_123457890",
                    IsTradingLocked = true,
                    ExchangeInfo = new ExchangeInfo { Id = 1, Name = "Binance" },
                },
                new ApiInfo()
                {
                    Id = "hbjadsghdqw67-2eyu21dygn-ui12dh",
                    Key = "Key2_123457890",
                    Secret = "Secret2_123457890",
                    IsTradingLocked = false,
                    ExchangeInfo = new ExchangeInfo { Id = 2, Name = "Kraken" },
                },
        };

        private readonly static List<ExchangeInfo> _ex = new List<ExchangeInfo>()
        {
            new ExchangeInfo { Id = 1, Name = "Binance"},
            new ExchangeInfo { Id = 2, Name = "Kraken"},
            new ExchangeInfo { Id = 3, Name = "Bitx"},
            new ExchangeInfo { Id = 4, Name = "CoinBase"},
        };
    }
}
