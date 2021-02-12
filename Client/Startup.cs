using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client
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
            services.AddAuthentication(Opt => 
            { 
                Opt.DefaultScheme = "_SID";
                Opt.DefaultChallengeScheme = "OAuth2.0_Server"; 
                
            })
                .AddCookie("_SID", opt=> { opt.Cookie.Name = "_SID"; })
                .AddOAuth("OAuth2.0_Server", options =>
                {
                    options.CallbackPath = Configuration["OAuth2.0:CallbackPath"];
                    options.ClientId = Configuration["OAuth2.0:ClientId"];
                    options.ClientSecret = Configuration["OAuth2.0:ClientSecret"];
                    options.AuthorizationEndpoint = Configuration["OAuth2.0:AuthorizationEndpoint"];
                    options.TokenEndpoint = Configuration["OAuth2.0:TokenEndpoint"];
                    options.Scope.Add("user");
                    options.Scope.Add("phone");
                    options.SaveTokens = true;
                    options.UsePkce = true;
                    options.Validate();
                });
            services.AddHttpClient();
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
               
                endpoints.MapDefaultControllerRoute();
                
            });
        }
    }
}
