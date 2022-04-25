using CryptoView.Web.Models.Connections;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CryptoView.Web.Interfaces
{
    public interface IConnectionsService
    {
        Task<IEnumerable<Connection>> GetConnectionsForUser(string userId);
        Task<Connection> GetConnectionById(string id);
        Task<IEnumerable<SelectListItem>> GetExchanges();

        Task<bool> CreateConnection(Connection connection);
        Task<bool> UpdateConnection(Connection connection);
        Task<bool> DeleteConnection(string id);

    }
}
