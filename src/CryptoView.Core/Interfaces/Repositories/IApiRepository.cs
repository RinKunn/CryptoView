using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using CryptoView.Core.Entities;

namespace CryptoView.Core.Interfaces.Repositories
{
    public interface IApiRepository
    {
        Task<IEnumerable<Api>> GetAllForUserAsync(string usedId);
        Task<Api> GetByIdAsync(string id, bool inclincludeDetailsudeInner = false);

        Task<Api> GetForExchange(int exchangeId, string userId, bool inclincludeDetailsudeInner = false);

        Task<Api> CreateAsync(Api connection);
        Task<Api> UpdateAsync(Api connection);
        Task DeleteAsync(string id);
    }
}
