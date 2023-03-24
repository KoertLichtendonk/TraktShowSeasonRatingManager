using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraktNet;
using TraktNet.Enums;
using TraktNet.Objects.Get.Ratings;
using TraktNet.Objects.Get.Seasons;
using TraktNet.Objects.Get.Shows;
using TraktNet.Parameters;
using TraktNet.Responses;
using TraktShowSeasonRatingManager.Configuration;
using TraktShowSeasonRatingManager.Helpers;

namespace TraktShowSeasonRatingManager.Factories
{
    public class GetFactory
    {
        private TraktClient _client;
        private TraktRateLimiter _rateLimiter;

        public GetFactory(TraktClient client)
        {
            _client = client;
            _rateLimiter = new TraktRateLimiter(_client, 1000, TimeSpan.FromMinutes(5));
        }

        public async Task<List<ITraktRatingsItem>> GetRatedEpisodes(TraktClient client)
        {
            return _rateLimiter.ExecuteAsync(async client => await client.Users.GetRatingsAsync(ConfigManager.Instance.Data.Username, TraktRatingsItemType.Episode)).Result;
        }

        public async Task<List<ITraktRatingsItem>> GetRatedSeasons(TraktClient client)
        {
            return _rateLimiter.ExecuteAsync(async client => await client.Users.GetRatingsAsync(ConfigManager.Instance.Data.Username, TraktRatingsItemType.Season)).Result;
        }

        public async Task<List<ITraktRatingsItem>> GetRatedShows(TraktClient client)
        {
            return _rateLimiter.ExecuteAsync(async client => await client.Users.GetRatingsAsync(ConfigManager.Instance.Data.Username, TraktRatingsItemType.Show)).Result;
        }

        public async Task<TraktListResponse<ITraktSeason>> GetShowSeasons(TraktClient client, string showId)
        {
            // Get the specified show and all its seasons
            TraktResponse<TraktListResponse<ITraktSeason>> response = _rateLimiter.ExecuteAsync(async client => await client.Seasons.GetAllSeasonsAsync(showId, new TraktExtendedInfo() { Full = true })).Result;

            if (!response.IsSuccess)
            {
                // Handle the error here
                return null;
            }

            return response;
        }
    }
}
