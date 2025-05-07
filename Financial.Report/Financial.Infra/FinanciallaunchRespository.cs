using Financial.Domain.Dtos;
using Financial.Domain.Events;
using Financial.Infra.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

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
        public async Task<decimal> GetBalanceAsync()
        {
            var db = _redis.GetDatabase();
            var key = GetRedisBalanceKey();

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

         public async Task SaveBalanceAsync(decimal value)
        {
            var db = _redis.GetDatabase();
            var key = GetRedisBalanceKey();

            try
            {
                _logger.LogInformation($"FinanciallaunchRespository: SaveBalanceAsync: Value: {value}");

                await db.StringIncrementAsync(key, (long)(value * 100));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SaveLauchAsync(Financiallaunch value)
        {
            var db = _redis.GetDatabase();
            var key = GetRedisLauchKey();

            try
            {
                _logger.LogInformation($"FinanciallaunchRespository: SaveLauchAsync: Value: {JsonSerializer.Serialize(value)}");

                var existingLaunchesJson = await db.StringGetAsync(key);
                var launches = new List<Financiallaunch>();

                if (existingLaunchesJson.HasValue)
                {
                    launches = JsonSerializer.Deserialize<List<Financiallaunch>>(existingLaunchesJson);
                }

                launches.Add(value);

                var updatedLaunchesJson = JsonSerializer.Serialize(launches);
                await db.StringSetAsync(key, updatedLaunchesJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar o lançamento no Redis.");
                throw;
            }
        }

        public async Task UpdateLauchAsync(Financiallaunch value)
        {
            var db = _redis.GetDatabase();
            var key = GetRedisLauchKey(value);

            try
            {
                _logger.LogInformation($"FinanciallaunchRespository: SaveLauchAsync: Value: {JsonSerializer.Serialize(value)}");

                var existingLaunchesJson = await db.StringGetAsync(key);
                var launches = new List<Financiallaunch>();

                if (existingLaunchesJson.HasValue)
                {
                    launches = JsonSerializer.Deserialize<List<Financiallaunch>>(existingLaunchesJson);
                }


                foreach (var item in launches)
                {
                    if(item.IdempotencyKey == value.IdempotencyKey)
                    {
                        item.Status = 2;

                        break;
                    }
                }

                var updatedLaunchesJson = JsonSerializer.Serialize(launches);
                await db.StringSetAsync(key, updatedLaunchesJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar o lançamento no Redis.");
                throw;
            }
        }


        public async Task<List<FinanciallaunchDto>> GetLauchAsync()
        {
            var db = _redis.GetDatabase();
            var key = GetRedisLauchKey();

            try
            {
                _logger.LogInformation($"FinanciallaunchRespository: GetLauchAsync");

                var existingLaunchesJson = await db.StringGetAsync(key);
                
                var launches = new List<FinanciallaunchDto>();

                if (existingLaunchesJson.HasValue)
                {
                    launches = JsonSerializer.Deserialize<List<FinanciallaunchDto>>(existingLaunchesJson);
                }

                return launches;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar o lançamento no Redis.");
                throw;
            }
        }


        private string GetRedisBalanceKey()
        {
            var date = DateTime.UtcNow;
            return $"Balance_{date.ToString("yyyy-MM-dd")}";
        }
        private string GetRedisLauchKey(Financiallaunch? financiallaunch = null)
        {
            var date = financiallaunch != null ? financiallaunch.CreateDate : DateTime.UtcNow;
            return $"Lauch_{date.ToString("yyyy-MM-dd")}";
        }

    }
}
