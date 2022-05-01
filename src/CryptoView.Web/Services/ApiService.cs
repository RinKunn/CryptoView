using CryptoView.Core.Entities;
using CryptoView.Core.Interfaces.Repositories;
using CryptoView.Web.Interfaces;
using CryptoView.Web.Models.ApiManagement;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CryptoView.Web.Services
{
    public class ApiService : IApiService
    {
        private readonly IExchangeRepository _exchangeRepository;
        private readonly IApiRepository _apiRepository;

        public ApiService(IExchangeRepository exchangeRepository, IApiRepository apiRepository)
        {
            _exchangeRepository = exchangeRepository;
            _apiRepository = apiRepository;
        }

        public async Task CreateApi(ApiInfo api, string userId)
        {
            var model = MapToEntity(api, userId);
            await _apiRepository.CreateAsync(model);
        }

        public async Task DeleteApi(string apiId, string userId)
        {
            await _apiRepository.DeleteAsync(apiId);
        }

        public Task<IEnumerable<ExchangeInfo>> ExchangesWithNoAPI(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiInfo> GetApiForExchange(int exchangeId, string userId)
        {
            var entity = await _apiRepository.GetForExchange(exchangeId, userId);
            return Map(entity);
        }

        public async Task<IEnumerable<ApiInfo>> GetApiList(string userId)
        {
            var entities = await _apiRepository.GetAllForUserAsync(userId);
            return entities?.Select(e => Map(e));
        }

        public async Task<ExchangeInfo> GetExchangeById(int id)
        {
            var entity = await _exchangeRepository.GetById(id);
            return Map(entity);
        }

        public async Task UpdateApi(ApiInfo api, string userId)
        {
            await _apiRepository.UpdateAsync(MapToEntity(api, userId));
        }


        private Api MapToEntity(ApiInfo apiInfo, string userId)
        {
            return new Api
            {
                Id = apiInfo.Id,
                Key = apiInfo.Key,
                Secret = apiInfo.Secret,
                IsTradingLocked = apiInfo.IsTradingLocked,
                UserId = userId,
                ExchangeId = apiInfo.ExchangeInfo.Id,
            };
        }

        private ApiInfo Map(Api api)
        {
            return new ApiInfo
            {
                Id = api.Id,
                Key = api.Key,
                Secret = api.Secret,
                IsTradingLocked = api.IsTradingLocked,
            };
        }


        private ExchangeInfo? Map(Exchange exchange)
            => exchange != null ? new ExchangeInfo
            {
                Id = exchange.Id,
                Name = exchange.Name,
                ImageUrl = exchange.ImgUrl,
            } : null;

        
    }
}
