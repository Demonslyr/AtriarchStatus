using AtriarchStatus.StatusClients.StarCitizen;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace AtriarchStatus
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<StarCitizenStatus>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseResponseCaching();

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(15)
                    };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                    new string[] { "Accept-Encoding" };

                await next();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/StarCitizen/GlobalStatus", async context =>
                {
                    var resultString = "Failed to get status.";
                    try
                    {
                        var scStatus = context.RequestServices.GetRequiredService<StarCitizenStatus>();
                        var statusResult = await scStatus.GetGlobalStatus();
                        if (!string.IsNullOrWhiteSpace(statusResult))
                            resultString = statusResult;
                    }
                    finally
                    {
                        await context.Response.WriteAsync(resultString);
                    }
                });
            });
        }
    }
}
