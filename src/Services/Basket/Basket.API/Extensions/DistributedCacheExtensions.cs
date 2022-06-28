using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Basket.API.Extensions
{
    public static class DistributedCacheExtensions
    {
        // To Set Cache
        public static async Task SetRecordAsync<T>( this IDistributedCache cache, string recordId, T data,
            TimeSpan? absoluteExpireTime = null,
            TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(120); // chache expire time
            options.SlidingExpiration = unusedExpireTime ?? TimeSpan.FromSeconds(300);  // if the cache is not used in the given time then the cache is expired
                                                                                        // even if does not met the absolute expiration

            var jsonData = JsonSerializer.Serialize(data);   // whatever the type of data should be serialize as Json   
            await cache.SetStringAsync(recordId, jsonData, options);   // key, value, attributes
        }



        // To Get Cache
        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetAsync(recordId);

            if(jsonData is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
