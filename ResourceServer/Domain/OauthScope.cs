using System;
using System.Collections.Generic;

#nullable disable

namespace ResourceServer.Domain
{
    public partial class OauthScope
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? OauthClientsId { get; set; }

        public virtual OauthClient OauthClients { get; set; }
    }
}
