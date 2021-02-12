namespace AuthorizationServer.Interfaces
{
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface ITokenGenerator
    {
        public Task<string> GenerateAccessToken(IdentityUser user);
        public string GenerateRefreshToken(IdentityUser user);
        public ClaimsPrincipal DecodeToken(string token);
        public bool Validate(string token);
    }
}
