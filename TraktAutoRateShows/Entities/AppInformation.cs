using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraktShowSeasonRatingManager.Entities
{
    public class AppInformation
    {
        public string refresh_uri { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }

        public AppInformation()
        {
            this.refresh_uri = String.Empty;
            this.client_id = String.Empty;
            this.client_secret = String.Empty;
        }
    }
}
