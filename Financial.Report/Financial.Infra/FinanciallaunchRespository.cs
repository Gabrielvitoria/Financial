using Financial.Infra.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Globalization;

namespace Financial.Infra
{
    public class FinanciallaunchRespository : IFinanciallaunchRespository
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<FinanciallaunchRespository> _logger;



        public FinanciallaunchRespository(IConfiguration configuration, IDistributedCache distributedCache, IConnectionMultiplexer redis, ILogger<FinanciallaunchRespository> logger)
        {
            _configuration = configuration;
            _distributedCache = distributedCache;
            _redis = redis;
            _logger = logger;
        }
        public async Task<decimal> GetAsync()
        {
            var db = _redis.GetDatabase();
            var key = GetRedisKey();

            var result = await db.StringGetAsync(key);

            if (result.HasValue)
            {
                if (long.TryParse(result, out long longValue)) // Tentar converter para long
                {
                    return longValue / 100.0m; // Dividir por 100.0m para obter decimal
                }
                else
                {
                    return 0.0m; // Tratar erro de conversão
                }
            }
            else
            {
                return 0.0m;
            }
        }

        public async Task SaveAsync(decimal value)
        {
            var db = _redis.GetDatabase();
            var key = GetRedisKey();

            try
            {
                _logger.LogInformation($"FinanciallaunchRespository: SaveAsync: Value: {value}");

                await db.StringIncrementAsync(key, (long)(value * 100));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetRedisKey()
        {
            var date = DateTime.UtcNow;
            return date.ToString("yyyy-MM-dd");
        }


    }
}
