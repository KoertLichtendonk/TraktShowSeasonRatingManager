using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraktNet;
using TraktNet.Objects.Get.Ratings;
using TraktNet.Objects.Get.Shows;
using TraktNet.Objects.Post.Syncs.Ratings;
using TraktNet.Objects.Post.Syncs.Ratings.Responses;
using TraktNet.PostBuilder;
using TraktNet.Responses;
using TraktShowSeasonRatingManager.Helpers;

namespace TraktShowSeasonRatingManager.Factories
{
    public class PostFactory
    {
        private TraktClient _client;
        private TraktRateLimiter _rateLimiter;

        public PostFactory(TraktClient client)
        {
            _client = client;
            _rateLimiter = new TraktRateLimiter(_client, 1, TimeSpan.FromSeconds(1));
        }

        public async Task<TraktResponse<ITraktSyncRatingsPostResponse>> SetRatings(TraktClient client, List<TraktSyncRatingsPostShow> showRatings, List<TraktSyncRatingsPostSeason> seasonRatings)
        {
            TraktSyncRatingsPost ratingsPost = new TraktSyncRatingsPost();
            ratingsPost.Shows = showRatings;
            ratingsPost.Seasons = seasonRatings;

            return _rateLimiter.ExecuteAsync(async client => client.Sync.AddRatingsAsync(ratingsPost).Result).Result;
        }
    }
}
