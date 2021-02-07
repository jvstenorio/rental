using System;
using System.Threading.Tasks;

namespace Rental.Domain.Repositories
{
    public interface IMemoryCacheRepository
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> getItem);
        void Remove<T>(string key);
    }
}
