using TraktNet;
using TraktNet.Enums;
using TraktNet.Objects.Authentication;
using TraktNet.Objects.Get.Ratings;
using TraktNet.Objects.Get.Seasons;
using TraktNet.Objects.Get.Shows;
using TraktNet.Objects.Post.Syncs.Ratings;
using TraktNet.Responses;
using TraktShowSeasonRatingManager.Configuration;
using TraktShowSeasonRatingManager.Extensions;
using TraktShowSeasonRatingManager.Factories;

var Config = ConfigManager.Instance.Data;

if(!Config.Enabled)
{
    return;
}

TraktClient client = new TraktClient(Config.AppInformation.client_id, Config.AppInformation.client_id)
{
    Authorization = TraktAuthorization.CreateWith(Config.Token.access_token)
};

if (!Config.Token.IsTokenValid())
{
    TraktResponse<ITraktAuthorization> newAuthorization = await client.Authentication.RefreshAuthorizationAsync(Config.Token.refresh_token);

    if (newAuthorization != null && newAuthorization.IsSuccess)
    {
        Config.Token.access_token = newAuthorization.Value.AccessToken;
        Config.Token.token_type = newAuthorization.Value.TokenType.ToString().ToLowerInvariant();
        Config.Token.expires_in = newAuthorization.Value.ExpiresInSeconds;
        Config.Token.refresh_token = newAuthorization.Value.RefreshToken;
        Config.Token.scope = newAuthorization.Value.Scope.ToString().ToLowerInvariant();
        Config.Token.created_at = (uint)((DateTimeOffset)newAuthorization.Value.CreatedAt).ToUnixTimeSeconds();

        client.Authorization = newAuthorization.Value;

        ConfigManager.Instance.Save();
    }
}

Console.WriteLine($"Requests without Authorization possible: {client.IsValidForUseWithoutAuthorization}");
Console.WriteLine($"Authentication possible: {client.IsValidForAuthenticationProcess}");
Console.WriteLine($"Requests with Authorization possible: {client.IsValidForUseWithAuthorization}");

GetFactory getFactory = new GetFactory(client);
var ratedEpisodes = await getFactory.GetRatedEpisodes(client);
var ratedSeasons = await getFactory.GetRatedSeasons(client);
var ratedShows = await getFactory.GetRatedShows(client);

var groupedEpisodes = ratedEpisodes
    .GroupBy(e => e.Show.Ids.Trakt) // Group episodes by the show's Trakt ID
    .ToDictionary(
        g => g.First().Show, // Use the show as the key for the outer dictionary
        g => g.GroupBy(e => e.Episode.SeasonNumber) // Group episodes within the show by the season number
            .ToDictionary(
                gg => gg.Key, // Use the season number as the key for the inner dictionary
                gg => gg.ToList() // Create a list of episodes for each season
            )
    );

List<TraktSyncRatingsPostShow> showRatings = new List<TraktSyncRatingsPostShow>();
List<TraktSyncRatingsPostSeason> seasonRatings = new List<TraktSyncRatingsPostSeason>();

foreach (KeyValuePair<ITraktShow, Dictionary<int?, List<ITraktRatingsItem>>> shows in groupedEpisodes)
{
    var show = shows.Key;
    List<ITraktSeason> showSeasons = await getFactory.GetShowSeasons(client, show.Ids.Trakt.ToString());
    List<double> episodeRatings = new List<double>();
    foreach (KeyValuePair<int?, List<ITraktRatingsItem>> seasons in shows.Value)
    {
        var season = seasons.Key;
        if (season.HasValue)
        {
            var seasonEpisodeList = seasons.Value;
            double? averageRating = seasonEpisodeList
                .Select(e => e.Rating)
                .Average();

            if (averageRating.HasValue)
            {
                episodeRatings.Add(averageRating.Value);

                int roundedAvgRating = (int)Math.Round(averageRating.Value, 0, MidpointRounding.ToEven);

                ITraktRatingsItem? currentSeasonRating = ratedSeasons.GetSeason(show.Ids.Trakt, (int)season);
                if (currentSeasonRating != null && currentSeasonRating.Rating != roundedAvgRating)
                {
                    var seasonObj = showSeasons.Single(i => i.Number == season);

                    seasonRatings.Add(new TraktSyncRatingsPostSeason() { Ids = seasonObj.Ids, Rating = roundedAvgRating });
                }
            }
        }
    }

    double averageShowRating = episodeRatings.Average();
    int roundedAvgShowRating = (int)Math.Round(averageShowRating, 0, MidpointRounding.ToEven);

    ITraktRatingsItem? currentShowRating = ratedShows.GetShow(show.Ids.Trakt);
    if (currentShowRating != null && currentShowRating.Rating != roundedAvgShowRating)
    {
        showRatings.Add(new TraktSyncRatingsPostShow() { Ids = show.Ids, Rating = roundedAvgShowRating });
    }
}

PostFactory postFactory = new PostFactory(client);
postFactory.SetRatings(client, showRatings, seasonRatings);