using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace foodOrderingApp.services.Redis
{
    public class RedisHealthCheckService : BackgroundService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisHealthCheckService> _logger;
        private bool _isRedisAvailable;

        public RedisHealthCheckService(IDistributedCache cache, ILogger<RedisHealthCheckService> logger)
        {
            _cache = cache;
            _logger = logger;
            _isRedisAvailable = cache != null;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Check Redis availability
                _isRedisAvailable = await IsRedisAvailable();

                if (_isRedisAvailable)
                {
                    _logger.LogInformation("Redis is up and running.");
                }
                else
                {
                    _logger.LogWarning("Redis is down. Will retry...");
                }

                // Wait for 10 seconds before next check
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task<bool> IsRedisAvailable()
        {
            try
            {
                // Trying to ping Redis to check if it's alive
                var ping = await _cache.GetStringAsync("ping");
                return ping != null;  // Simple check, if ping exists then Redis is up
            }
            catch
            {
                return false;
            }
        }

        public bool IsRedisAvailableStatus() => _isRedisAvailable;
    }
}
