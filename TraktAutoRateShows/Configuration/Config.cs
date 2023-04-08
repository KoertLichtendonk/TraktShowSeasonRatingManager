using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraktShowSeasonRatingManager.Entities;
using System.Linq;
using QuickConfig;

namespace TraktShowSeasonRatingManager.Configuration
{
    public class Config : ConfigBase
    {
        public OauthToken Token { get; set; }
        public AppInformation AppInformation { get; set; }
        public string Username { get; set; }
        public bool Enabled { get; set; }

        public Config()
        {
            this.Token = new OauthToken();
            this.AppInformation = new AppInformation();
            this.Username = String.Empty;
            this.Enabled = false;
        }
    }
}
