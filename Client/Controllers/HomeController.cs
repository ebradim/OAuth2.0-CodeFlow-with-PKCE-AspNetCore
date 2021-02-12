using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Client.SharedData;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory httpClient;

        public HomeController(ILogger<HomeController> logger,IHttpClientFactory httpClient)
        {
            _logger = logger;
            this.httpClient = httpClient;
        }

        public async Task<IActionResult> IndexAsync()
        {
            
            var accesstoken = await HttpContext.GetTokenAsync("access_token");
            var refreshtoken = await HttpContext.GetTokenAsync("refresh_token");
            if(accesstoken is not null || accesstoken?.Length > 0)
            {
                var client = httpClient.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accesstoken}");
                //send token to api
                var api = await client.GetAsync(
                    "https://localhost:44392/api/public/accounts");
                    var a = await api.Content.ReadAsStringAsync();
                if (api.IsSuccessStatusCode)
                {
                    ViewData["auth"] = a;
                    StaticData.IsAuth = true;
                    return View(model:a);
                }
                else
                {
                    StaticData.IsAuth = false;

                }
                return View();

            }
            else
            {

                StaticData.IsAuth = false;
                return View();
            }
           
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return RedirectToAction("Index");
            //var token = await HttpContext.GetTokenAsync("access_token");

            //var client = httpClient.CreateClient();
            //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            ////send token to api
            //var api  = await client.GetAsync(
            //        "https://localhost:44392/api/public/accounts");

            //if (api.IsSuccessStatusCode)
            //{
            //    var a = await api.Content.ReadAsStringAsync();
            //    ViewData["auth"] = a;
            //    StaticData.IsAuth = true;
              
            //}
            //else
            //{
            //    StaticData.IsAuth = false;
            //    return View();
            //}

           
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
