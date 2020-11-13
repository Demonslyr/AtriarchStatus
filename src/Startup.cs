using AtriarchStatus.StatusClients.StarCitizen;
using AtriarchStatus.StatusClients.WorldOfWarcraft;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace AtriarchStatus
{
    public class HtmlOutputFormatter : StringOutputFormatter
    {
        public HtmlOutputFormatter()
        {
            SupportedMediaTypes.Add("text/html");
        }
    }
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddHttpClient();
            services.AddSingleton<StarCitizenStatus>();
            services.AddSingleton<IcecrownRaresStatus>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Healthy"); })
                endpoints.MapGet("/StarCitizen/GlobalStatus", async context =>
                {
                    var resultString = "Failed to get status.";
                    try
                    {
                        var cache = context.RequestServices.GetRequiredService<IMemoryCache>();
                        var cacheEntry = await cache.GetOrCreateAsync<string>("StarCitizen/GlobalStatus", async entry =>
                        {
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15);
                            var scStatus = context.RequestServices.GetRequiredService<StarCitizenStatus>();
                            var statusResult = await scStatus.GetGlobalStatus();
                            return string.IsNullOrWhiteSpace(statusResult)
                                ? "Failed to get status"
                                : statusResult;
                        });
                        resultString = cacheEntry;
                    }
                    finally
                    {
                        await context.Response.WriteAsync(resultString);
                    }
                });
                endpoints.MapGet("/wow/icrares", async context =>
                {
                    var resultString = "Failed to get status";
                    try
                    {
                        var cache = context.RequestServices.GetRequiredService<IMemoryCache>();
                        var cacheEntry = cache.GetOrCreate<string>("wow/icrares", entry =>
                        {
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15);
                            var icrares = context.RequestServices.GetRequiredService<IcecrownRaresStatus>();
                            var statusResult = icrares.GetUpcomingRare();
                            return string.IsNullOrWhiteSpace(statusResult)
                                ? "Failed to get status"
                                : statusResult;
                        });
                        resultString = cacheEntry;
                    }
                    finally
                    {
                        context.Response.ContentType = "html";
                        await context.Response.WriteAsync(resultString);
                    }
                });
            });
        }
    }
}
