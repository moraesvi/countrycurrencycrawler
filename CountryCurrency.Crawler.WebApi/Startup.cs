using CountryCurrency.Crawler.Common;
using CountryCurrency.Crawler.Domain.Interface.Uol;
using CountryCurrency.Crawler.Logic;
using CountryCurrency.Crawler.Parse;
using CountryCurrency.Crawler.WebApi.Controllers;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace CountryCurrency.Crawler.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<ICountriesParse>(s => new CountriesParse(new HtmlDocument()));
            services.AddSingleton<IUolCurrencyCrawler, UolCurrencyCrawler>();
            services.AddSingleton<IUolCurrencyCrawlerCache, UolCurrencyCrawlerCache>();
            services.AddSingleton<IUolCurrencyCrawlerBuilder, UolCurrencyCrawlerBuilder>();
            services.AddControllers();
            services.AddLogging();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                Exception exception = context.Features
                                             .Get<IExceptionHandlerPathFeature>()
                                             .Error;

                CommonResult response = new CommonResult().ApiResult(context.RequestServices.GetRequiredService<ILogger<CustomControllerBase>>(), exception, env.IsDevelopment());

                await context.Response.WriteAsJsonAsync(response);
            }));

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
