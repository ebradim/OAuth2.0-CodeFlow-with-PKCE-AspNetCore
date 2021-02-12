namespace AuthorizationServer.Controllers
{
    using AuthorizationServer.AuthSchemes;
    using AuthorizationServer.Interfaces;
    using AuthorizationServer.Models;
    using AuthorizationServer.Persistence;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
 
    public class OAuthController : Controller
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly DataContext dataContext;
        private readonly ITokenGenerator tokenGenerator;
        public OAuthController(SignInManager<IdentityUser> signInManager,DataContext dataContext,
            ITokenGenerator tokenGenerator)
        {
            this.signInManager = signInManager;
            this.dataContext = dataContext;
            this.tokenGenerator = tokenGenerator;
        }
        [HttpGet]
        public async Task<IActionResult> AuthorizeAsync(
            string client_id,
            string scope,
            string response_type,
            string redirect_uri,
            string state,
            string code_challenge,
            string code_challenge_method
            )
        {
            var clientid = Guid.Parse(client_id);
            var client = await dataContext.OAuthClients
                .Where(x => x.ClientId == clientid)
                .Include(x=>x.OAuthScopes)
                .FirstOrDefaultAsync();
            if(client is not null)
            {
                //since the first is user, and the second targets the data to retrieve
                var scope_target = client.OAuthScopes.Last().Name;
                if (scope.EndsWith(scope_target))
                {
                    ViewData.Add("AppName", client.AppName);
                    ViewData.Add("Scope", scope_target);
                    ViewData.Add("AppWebsite", client.Website);
                    var queryBuilder = new QueryBuilder();
                    queryBuilder.Add("redirect_uri", redirect_uri);
                    queryBuilder.Add("scope", scope);
                    queryBuilder.Add("state", state);


                    TempData.Add("query", queryBuilder.ToString());
                    return View();
                }
              
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });


        }
        [HttpPost]
        public async Task<IActionResult> AuthorizeAsync(
            AuthUser authLogin,
            string redirect_uri,
            string state,
            string scope
            )
        {
            var user = await dataContext.Users
                .Where(x => x.Email == authLogin.Email)
                .AsNoTracking().FirstOrDefaultAsync();

            var success = await signInManager.CheckPasswordSignInAsync(user, authLogin.Password,false);
            if (success.Succeeded)
            {
              
                string auth_code = Guid.NewGuid().ToString();
                var queryBuilder = new QueryBuilder
                {
                    { "code", auth_code },
                    { "state", state },
                    { "scope",scope }
                };
                StaticData.Scope = scope;
                StaticData.CurrentUserName = user.UserName;
                return Redirect($"{redirect_uri}{queryBuilder}");
            }
            else
            {
                TempData.Add("LoginError", "Email or Password is wrong");
                return View("Authorize", authLogin);
            }

          
        }
        public async Task<IActionResult> TokenAsync(
            string grant_type,
            string code, 
            string redirect_uri,
            string client_id,
            string code_verifier,
            string client_secret
            )
        {
            var clientid = Guid.Parse(client_id);
            var clientsecret = Guid.Parse(client_secret);
            var client = await dataContext.OAuthClients.Where(x => x.ClientId == clientid &&
            x.ClientSecret == clientsecret).AsNoTracking().FirstOrDefaultAsync();
            if(client is not null)
            {

                var user = await dataContext.Users.Where(x => x.UserName == StaticData.CurrentUserName).AsNoTracking().FirstOrDefaultAsync();
                var access_token = await tokenGenerator.GenerateAccessToken(user);
                var refresh_token = tokenGenerator.GenerateRefreshToken(user);
                var responseObject = new
                {
                    access_token,
                    refresh_token,
                    token_type = "bearer"
                };
                var responseJson = JsonConvert.SerializeObject(responseObject);
                var responseBytes = Encoding.UTF8.GetBytes(responseJson);

                await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);

                return Redirect(redirect_uri);
            }
            return View();
        }
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public IActionResult ValidateAsync(string access_token)
        {
            
            var validation  = tokenGenerator.Validate(access_token);
            return validation ? Ok() : Forbid();

        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("oauth/scope")]
        public IActionResult GetScope(string access_token)
        {
            var scope = tokenGenerator.DecodeToken(access_token).Claims.First(x => x.Type == "scope").Value;
            var username = tokenGenerator.DecodeToken(access_token).Claims.First(x => x.Type == "_unid").Value;
            return Ok(new { scope,username});
        }

    }
}
