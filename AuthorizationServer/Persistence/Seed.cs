namespace AuthorizationServer.Persistence
{
    using AuthorizationServer.Domain;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Seed
    {
        /// <summary>
        /// This method is used for Demo/Development puprose only in order to have some data
        /// </summary>
        /// <param name="userManager">UserManager that configure IdentityUser</param>
        /// <param name="roleManager">RoleManager that configre IdentityRole</param>
        /// <returns>Completed Task</returns>
        public static async Task AddInitialUsersAsync(DataContext dataContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            #region Seeding Users for demo purpose
            if (!await dataContext.Users.AnyAsync())
            {
                // add users to database
                var users = new List<IdentityUser>
                {
                    new IdentityUser
                    {
                        UserName = "ebrahim",
                        Email="ebrahim@test.com",
                        PhoneNumber = "654124820",
                    },
                    new IdentityUser
                    {
                        UserName = "admin",
                        Email="admin@test.com",
                        PhoneNumber = "147852369"
                    },
                    new IdentityUser
                    {
                        UserName="coolguy",
                        Email="cool_guy@test.com",
                        PhoneNumber = "147896325"
                    }
                };
                // add some roles
                var roles = new List<IdentityRole>
                {
                    new IdentityRole
                    {
                        Name="BetaUser"
                    },
                    new IdentityRole
                    {
                        Name = "SuperUser"
                    },
                    new IdentityRole
                    {
                        Name ="Moderator"
                    }
                };
                // create roles first
                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
                // create password with UserManager to get Hashed and assign to roles
                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "demoauth");
                    await userManager.AddToRoleAsync(user, "BetaUser");
                }
            }
            #endregion

            #region Seeding OAuthClients with Scopes instead of creating Developer Page(developer.yoursite.com)
            if (!await dataContext.OAuthClients.AnyAsync())
            {
                var clients = new List<OAuthClients>
                {
                    new OAuthClients
                    {
                        ClientSecret = Guid.NewGuid(),
                        ClientId = Guid.NewGuid(),
                        AppName ="OnlineMovies",
                        FallbackUri = "/oauth/callback",
                        Website = "https://localhost:44336",
                        OAuthScopes = new List<OAuthScope>
                        {
                            new OAuthScope
                            {
                                Name = "user"
                            },
                            new OAuthScope
                            {
                                Name = "name"
                            }
                        },
                    },
                    new OAuthClients
                    {
                        ClientSecret = Guid.NewGuid(),
                        ClientId = Guid.NewGuid(),
                        AppName ="SmartNotes",
                        FallbackUri = "/oauth/callback",
                        Website = "https://localhost:44336",
                        OAuthScopes = new List<OAuthScope>
                        {
                            new OAuthScope
                            {
                                Name = "user"
                            },
                            new OAuthScope
                            {
                                Name = "email"
                            }
                        },
                    }
                };

                await dataContext.OAuthClients.AddRangeAsync(clients);
                await dataContext.SaveChangesAsync();
            } 
            #endregion
        }
    }
}
