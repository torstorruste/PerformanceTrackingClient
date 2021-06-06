using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PerformanceClient.Service;
using System.Collections;

namespace PerformanceClient
{
    public class Program
    {
        private static string BASE_URL = "performance.superhelt.org:8080";

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped<IPlayerService, PlayerService>();
            builder.Services.AddScoped<IStatisticsService, StatisticsService>();
            builder.Services.AddScoped<IBossService, BossService>();
            builder.Services.AddScoped<IMeasureService, MeasureService>();
            builder.Services.AddScoped<IRankingService, RankingService>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri($"http://{BASE_URL}/performance/") });

            await builder.Build().RunAsync();
        }
    }
}
