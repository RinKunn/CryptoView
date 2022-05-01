using CryptoView.Web.Models.ApiManagement;

namespace CryptoView.Web.Interfaces
{
    public interface IApiService
    {
        /// <summary>
        /// Returns user API for certain crypto exchange by given exchange id <br/>
        /// </summary>
        /// <param name="exchangeId">Exchange ID</param>
        /// <param name="userId">User ID</param>
        /// <returns>API with <see cref="ExchangeInfo"/> if user has it for exchnage, otherwise null</returns>
        Task<ApiInfo> GetApiForExchange(int exchangeId, string userId);

        /// <summary>
        /// Returns a list of user APIs for crypto exchanges
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of APIs with initialized <see cref="ExchangeInfo"/></returns>
        Task<IEnumerable<ApiInfo>> GetApiList(string userId);

        /// <summary>
        /// Returns a list of crypto exchanges for which the user doesn't have an API
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of Exchanges</returns>
        Task<IEnumerable<ExchangeInfo>> ExchangesWithNoAPI(string userId);

        /// <summary>
        /// Returns exchange by given <paramref name="id"/>
        /// </summary>
        /// <param name="id">Exchange's ID</param>
        /// <returns>Exchange</returns>
        Task<ExchangeInfo> GetExchangeById(int id);

        /// <summary>
        /// Creates API for user with <paramref name="userId"/>
        /// </summary>
        /// <param name="apiInfo">API</param>
        /// <param name="userId">User ID</param>
        Task CreateApi(ApiInfo apiInfo, string userId);

        /// <summary>
        /// Updates API for user with <paramref name="userId"/> if it's available for him
        /// </summary>
        /// <param name="apiInfo">API</param>
        /// <param name="userId">User ID</param>
        Task UpdateApi(ApiInfo apiInfo, string userId);

        /// <summary>
        /// Deletes API for user with <paramref name="userId"/> if it's available for him
        /// </summary>
        /// <param name="apiId">API ID</param>
        /// <param name="userId">User ID</param>
        Task DeleteApi(string apiId, string userId);
    }
}
