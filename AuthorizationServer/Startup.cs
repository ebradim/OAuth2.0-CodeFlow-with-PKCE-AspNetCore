using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthorizationServer.AuthSchemes;
using AuthorizationServer.Interfaces;
using AuthorizationServer.Persistence;
using AuthorizationServer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace AuthorizationServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["secret_key"]));

                services.AddAuthentication(options=> 
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options=> 
                {

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = key,
                        SaveSigninToken = true,
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidIssuer = "ebrahim.auth.com",
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Query.ContainsKey("access_token"))
                            {
                                context.Token = context.Request.Query["access_token"];
                            }

                            return Task.CompletedTask;
                        },
                        
                    };
                });

            //Add Sqlite DataBase for demo purpose only.
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("SqliteDb"));
            });
            //Configure the default Identity settings.
            //I will configure password only for demo purpose only.
            var identity = services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                var passwordManager = options.Password;           
                passwordManager.RequireDigit = false;
                passwordManager.RequireLowercase = false;
                passwordManager.RequireNonAlphanumeric = false;
                passwordManager.RequireUppercase = false;            
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddUserManager<UserManager<IdentityUser>>()
            .AddSignInManager<SignInManager<IdentityUser>>()
            .AddRoleValidator<RoleValidator<IdentityRole>>()
            .AddRoleManager<RoleManager<IdentityRole>>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddControllersWithViews();

         
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
