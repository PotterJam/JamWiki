using System;
using System.Text;
using JamWiki.Api.Config;
using JamWiki.Api.Users;
using JamWiki.Api.Wikis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace JamWiki.Api
{
    public class Startup
    {
        private IConfigurationRoot Configuration { get; }
        
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        
        private PostgresConfiguration PostgresConfiguration => new PostgresConfiguration
        {
            Database = Configuration.GetValue<string>("DB_NAME"),
            Host = Configuration.GetValue<string>("DB_HOST"),
            Port = Configuration.GetValue<int>("DB_PORT"),
            Username = Configuration.GetValue<string>("DB_USER_NAME"),
            Password = Configuration.GetValue<string>("DB_PASSWORD"),
        };
        
        private SecurityConfiguration SecurityConfiguration => new SecurityConfiguration
        {
            JwtSigningKey = Configuration.GetValue<string>("JWT_SIGNING_KEY"),
            UserCredentialKey = Configuration.GetValue<string>("USER_CREDENTIALS_KEY")
        };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;

                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityConfiguration.JwtSigningKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddSingleton(PostgresConfiguration);
            services.AddSingleton(SecurityConfiguration);

            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IUserStore, UserStore>();
            services.AddSingleton<IWikiStore, WikiStore>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}