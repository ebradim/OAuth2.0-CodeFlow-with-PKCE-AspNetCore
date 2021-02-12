using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ResourceServer.Requirements
{
    public class AuthJwtRequirement : IAuthorizationRequirement 
    {

    }
    public class AuthJwtRequirementHandler : AuthorizationHandler<AuthJwtRequirement>
    {
        private readonly HttpClient client;
        private readonly HttpContext _httpContext;

        public AuthJwtRequirementHandler(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            client = httpClientFactory.CreateClient();
            _httpContext = httpContextAccessor.HttpContext;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AuthJwtRequirement requirement)
        {
            if (_httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var accessToken = authHeader.ToString().Split(' ')[1];

                var response = await client
                    .GetAsync($"https://localhost:44370/oauth/validate?access_token={accessToken}");
                var scope = await response.Content.ReadAsStringAsync();
                // scope might be email, etc..
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}