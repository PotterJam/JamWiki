using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WikiApi
{
    public class Startup
    {
        private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyMethod();
                        builder.AllowAnyHeader();
                    });
            });

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                options.SignIn.RequireConfirmedAccount = true);
            
            services.AddAuthentication().AddGoogle(options =>
                {
//                    IConfigurationSection googleAuthNSection =
//                        Configuration.GetSection("Authentication:Google");

                    options.ClientId = "357451785008-mvk7i4sla4qdq9nh8u5uml09g5gdqt26.apps.googleusercontent.com";// googleAuthNSection["ClientId"];
                    options.ClientSecret = "UQDtH6Fswg3Rmcjbyx5-xzrf"; // googleAuthNSection["ClientSecret"];
                });
            
            services.AddTransient<IWikiStore, WikiStore>();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            
            app.UseCors();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}