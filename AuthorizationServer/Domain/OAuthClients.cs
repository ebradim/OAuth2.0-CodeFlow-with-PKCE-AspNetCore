namespace AuthorizationServer.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class OAuthClients
    {
        public int Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid ClientSecret { get; set; }
        public string AppName { get; set; }
        public string Website { get; set; }
        public string FallbackUri { get; set; }
        public ICollection<OAuthScope> OAuthScopes { get; set; }
    }

    // Using Guid for clarification and Demo purpose only
}
