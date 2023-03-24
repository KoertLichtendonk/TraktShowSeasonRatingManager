using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraktNet;
using TraktNet.Exceptions;

namespace TraktShowSeasonRatingManager.Helpers
{
    public class TraktRateLimiter
    {
        private readonly TraktClient _client;
        private readonly SemaphoreSlim _rateLimiter;
        private readonly TimeSpan _rateLimitInterval;

        public TraktRateLimiter(TraktClient client, int maxRequestsPerInterval, TimeSpan rateLimitInterval)
        {
            _client = client;
            _rateLimiter = new SemaphoreSlim(maxRequestsPerInterval, maxRequestsPerInterval);
            _rateLimitInterval = rateLimitInterval;
        }

        public async Task<T> ExecuteAsync<T>(Func<TraktClient, Task<T>> action)
        {
            await _rateLimiter.WaitAsync();

            try
            {
                var result = await action(_client);
                return result;
            }
            catch (TraktRateLimitException ex)
            {
                await Task.Delay(ex.RetryAfter.GetValueOrDefault(300) * 1000);
                return await ExecuteAsync(action);
            }
            finally
            {
                _ = Task.Delay(_rateLimitInterval).ContinueWith(t => _rateLimiter.Release());
            }
        }
    }
}
