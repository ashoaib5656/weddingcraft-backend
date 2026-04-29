using StackExchange.Redis;
using weddingcraft_be.Interfaces.Services;

namespace weddingcraft_be.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _db;

        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            try
            {
                await _db.StringSetAsync(key, value, expiry, When.Always, CommandFlags.None);
            }
            catch (Exception ex)
            {
                throw new Exception("Redis service is currently unavailable.", ex);
            }
        }

        public async Task<string?> GetAsync(string key)
        {
            try
            {
                return await _db.StringGetAsync(key);
            }
            catch (Exception ex)
            {
                throw new Exception("Redis service is currently unavailable.", ex);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                throw new Exception("Redis service is currently unavailable.", ex);
            }
        }
    }
}
