using System;
using System.Collections.Generic;

#nullable disable

namespace ResourceServer.Domain
{
    public partial class OauthClient
    {
        public OauthClient()
        {
            OauthScopes = new HashSet<OauthScope>();
        }

        public long Id { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AppName { get; set; }
        public string FallbackUri { get; set; }
        public string Website { get; set; }

        public virtual ICollection<OauthScope> OauthScopes { get; set; }
    }
}
