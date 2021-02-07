using Microsoft.Extensions.Caching.Memory;
using Rental.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Rental.Infrastructure.Repositories
{
    public class MemoryCacheRepository : IMemoryCacheRepository
    {
        private readonly MemoryCache _cache;
        private readonly int _absoluteExpirationInSec;
        private readonly int _slidingExpirationInSec;

        public MemoryCacheRepository(int sizeLimit, int absoluteExpirationInSec, int slidingExpirationInSec)
        {
            _cache = new MemoryCache(new MemoryCacheOptions()
            {
                SizeLimit = sizeLimit
            });
            _absoluteExpirationInSec = absoluteExpirationInSec;
            _slidingExpirationInSec = slidingExpirationInSec;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> getItem)
        {
            try
            {
                T item;
                var internalKey = GetInternalKey<T>(key);
                if (!_cache.TryGetValue<T>(internalKey, out item))
                {
                    item = await getItem();
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        Size = 1,
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_absoluteExpirationInSec),
                        SlidingExpiration = TimeSpan.FromSeconds(_slidingExpirationInSec)
                    };
                    _cache.Set<T>(internalKey, item, cacheEntryOptions);
                }
                return item;
            }
            catch
            {
                return await getItem();
            }
        }

        public void Remove<T>(string key) => _cache.Remove(GetInternalKey<T>(key));

        private string GetInternalKey<T>(string key) => $"{typeof(T).FullName}:{key}";

    }
}
