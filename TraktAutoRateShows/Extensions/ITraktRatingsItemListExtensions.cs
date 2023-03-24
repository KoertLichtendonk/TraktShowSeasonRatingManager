using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraktNet.Objects.Get.Ratings;

namespace TraktShowSeasonRatingManager.Extensions
{
    public static class ITraktRatingsItemListExtensions
    {
        public static ITraktRatingsItem? GetSeason(this List<ITraktRatingsItem> list, uint traktShowId, int seasonNumber)
        {
            var season = list.Where(i => i.Show.Ids.Trakt == traktShowId && i.Season.Number == seasonNumber).FirstOrDefault();

            return season;
        }
        public static ITraktRatingsItem? GetShow(this List<ITraktRatingsItem> list, uint traktShowId)
        {
            var show = list.Where(i => i.Show.Ids.Trakt == traktShowId).FirstOrDefault();

            return show;
        }
    }
}
