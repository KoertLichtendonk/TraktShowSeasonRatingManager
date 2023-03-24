using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraktShowSeasonRatingManager.Entities
{
    public class OauthToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public uint expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public uint created_at { get; set; }

        public OauthToken()
        {
            this.access_token = String.Empty;
            this.token_type = String.Empty;
            this.expires_in = 0;
            this.refresh_token = String.Empty;
            this.scope = String.Empty;
            this.created_at = 0;
        }

        public DateTime GetExpirationDateTime()
        {
            DateTime createdAt = DateTimeOffset.FromUnixTimeSeconds(created_at).UtcDateTime;
            DateTime expirationDateTime = createdAt.AddSeconds(expires_in);
            return expirationDateTime;
        }

        public bool IsTokenValid()
        {
            DateTime expirationDateTime = GetExpirationDateTime();
            DateTime currentDateTime = DateTime.UtcNow;

            return currentDateTime < expirationDateTime;
        }
    }
}
