using CryptoView.Web.Interfaces;
using CryptoView.Web.Models.Connections;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CryptoView.Web.Services
{
    public class ConnectionsService : IConnectionsService
    {
        private readonly static List<Connection> _data = new List<Connection>()
        {
            new Connection()
                {
                    Id = "78781278-u12yy2-21yu21u721",
                    ExchangeName = "Binance",
                    ExchangeId = 1,
                    ApiKey = "API Key1_123457890",
                    ApiSecret = "API Secret1_123457890",
                    Created = DateTime.Now.AddDays(-10).AddHours(-2),
                    IsTradingLocked = true,
                    UserId = "39515008-7775-4937-a3dd-96129aeeb875"
                },
                new Connection()
                {
                    Id = "hbjadsghdqw67-2eyu21dygn-ui12dh",
                    ExchangeName = "Kraken",
                    ApiKey = "Key2_123457890",
                    ApiSecret = "Secret2_123457890",
                    ExchangeId = 2,
                    Created = DateTime.Now.AddDays(-4).AddMinutes(-213),
                    IsTradingLocked = false,
                    UserId = "39515008-7775-4937-a3dd-96129aeeb875"
                },
        };

        public Task<bool> CreateConnection(Connection connection)
        {
            connection.Id = Guid.NewGuid().ToString();
            connection.ExchangeName = connection.ExchangeId == 1 ? "Binance" : "Kraken";
            _data.Add(connection);
            return Task.FromResult(true);
        }

        public async Task<Connection> GetConnectionById(string id)
        {
            await Task.Delay(10);
            return _data.FirstOrDefault(d => d.Id == id);
        }

        public async Task<IEnumerable<Connection>> GetConnectionsForUser(string userId)
        {
            await Task.Delay(100);
            return _data;
        }

        public async Task<IEnumerable<SelectListItem>> GetExchanges()
        {
            await Task.Delay(10);
            return new List<SelectListItem>
            {
                new SelectListItem("Binance", "1"),
                new SelectListItem("Kraken", "2"),
            };
        }

        public async Task<bool> UpdateConnection(Connection connection)
        {
            await Task.Delay(100);
            var ind = _data.FindIndex(d => d.Id == connection.Id);
            if (ind == -1)
                return false;
            _data[ind] = connection;
            return true;
        }

        public async Task<bool> DeleteConnection(string id)
        {
            await Task.Delay(100);
            var ind = _data.FindIndex(d => d.Id == id);
            if (ind == -1)
                return false;
            _data.RemoveAt(ind);
            return true;
        }
    }
}
