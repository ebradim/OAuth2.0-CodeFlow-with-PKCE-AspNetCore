namespace AuthorizationServer.Services
{
    using AuthorizationServer.Interfaces;
    using AuthorizationServer.Persistence;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public class TokenGenerator : ITokenGenerator
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SymmetricSecurityKey key;
        private readonly IConfiguration configuration;
        public TokenGenerator(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["secret_key"]));
        }
        public async Task<string> GenerateAccessToken(IdentityUser user)
        {
            var claim = new List<Claim>
            {
                new Claim("_cuser",user.Id),
                new Claim("_unid",user.UserName),
                new Claim("scope",StaticData.Scope.Split(" ").Last())
            };
            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claim.Add(new Claim(ClaimTypes.Role, role));
            }

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var descriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = credentials,
                Subject = new ClaimsIdentity(claim),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = "ebrahim.auth.com",


            };
            var tokenHanlder = new JwtSecurityTokenHandler();
            var securityToken = tokenHanlder.CreateToken(descriptor);
            return tokenHanlder.WriteToken(securityToken);
        }


        public string GenerateRefreshToken(IdentityUser user)
        {
            var firstToken = $"{Guid.NewGuid()}-{DateTime.UtcNow}";
            var firstEncoded = Encoding.UTF8.GetBytes(firstToken);
            var firstPart = Convert.ToBase64String(firstEncoded);

            return firstPart + Guid.NewGuid().ToString() + "-" + Convert.ToBase64String(Encoding.UTF8.GetBytes(user.UserName));
        }

        public bool Validate(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler()
               .ValidateToken(token, new TokenValidationParameters
               {
                   IssuerSigningKey = key,
                   SaveSigninToken = true,
                   ValidateIssuer = true,
                   ValidateAudience = false,
                   ValidateLifetime = true,
                   ValidIssuer = "ebrahim.auth.com",
                   ClockSkew = TimeSpan.Zero
               }, out var securityToken);

                return securityToken.Issuer == "ebrahim.auth.com";
            }
            catch (Exception ex) when(ex is SecurityTokenExpiredException)
            {

                return false;
            }
        }
        public ClaimsPrincipal DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler()
                .ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = key,
                    SaveSigninToken = true,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidIssuer="ebrahim.auth.com",
                    ClockSkew = TimeSpan.Zero
                }, out var securityToken);
            
            return tokenHandler;
        }

    }
}
