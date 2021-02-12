using AuthorizationServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizationServer.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;

        public AccountController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }
        // To display register page
        [HttpGet]
        public IActionResult Register()
        {       
            return View();
        }
        [AcceptVerbs("Post")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(AuthUser user)
        {
            
            if (ModelState.IsValid)
            {
                await userManager.CreateAsync(new IdentityUser
                {
                    Email = user.Email,
                    UserName = user.UserName
                }, user.Password);
                
                return Ok();
            }
            else
            {
                return View("Register",user);
            }
        }
    }
}
