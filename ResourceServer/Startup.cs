using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ResourceServer.Domain;
using ResourceServer.Requirements;

namespace ResourceServer
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
            services.AddAuthentication(options=> 
            {
                options.DefaultChallengeScheme = "JwtAuth";
                options.DefaultAuthenticateScheme = "JwtAuth";
            });
            services.AddAuthorization(config =>
            {
                config.AddPolicy("authjwt", policy =>
                {
                    //policy.RequireClaim(ClaimTypes.Sid,"authToken");
                    policy.AddRequirements(new AuthJwtRequirement());
                });
               
            });
           
            services.AddHttpClient()
              .AddHttpContextAccessor();
            services.AddScoped<IAuthorizationHandler, AuthJwtRequirementHandler>();
            services.AddDbContext<AuthDemoDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("SqliteDb"));
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ResourceServer", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ResourceServer v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
