namespace AuthorizationServer.Persistence
{
    using AuthorizationServer.Domain;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<OAuthClients> OAuthClients { get; set; }
        public DbSet<OAuthScope> OAuthScope { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
