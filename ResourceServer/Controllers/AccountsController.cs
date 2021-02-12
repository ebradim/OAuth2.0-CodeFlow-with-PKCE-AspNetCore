using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ResourceServer.Models;

namespace ResourceServer.Controllers
{
    [ApiController]
    [Route("api/public/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IHttpClientFactory httpClient;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly AuthDemoDbContext dbContext;

        public AccountsController(IHttpClientFactory httpClient,IHttpContextAccessor contextAccessor,AuthDemoDbContext dbContext)
        {
            this.httpClient = httpClient;
            this.contextAccessor = contextAccessor;
            this.dbContext = dbContext;
        }
        [Authorize(Policy = "authjwt")]
        [HttpGet]
        public async Task<string> GetAsync()
        {
            // Because we are sending request to this point
            // Token is being send with Header 
            if (contextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                // Scheme: Bearer MYTOKEN
                // Take the last one
                var token = authHeader.ToString().Split(' ')[1];
                var client = httpClient.CreateClient();
                var response = await client.GetAsync($"https://localhost:44370/oauth/scope?access_token={token}");
                var result = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<ScopeResult>(result);
                //since we are dealing with only two scopes
                if(json.Scope == "phone")
                {
                    var email = await dbContext.AspNetUsers.Where(x => x.UserName == json.UserName).Select(x => x.PhoneNumber).FirstOrDefaultAsync();
                    return email;
                }
                else
                {
                    var userName = await dbContext.AspNetUsers.Where(x => x.UserName == json.UserName).Select(x => x.UserName).FirstOrDefaultAsync();
                    return userName;
                }
            }
            else
            {
                throw new Exception("Unauthorized");
            }
            
        }
    }
}
