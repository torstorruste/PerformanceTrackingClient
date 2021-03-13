using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PerformanceClient.Model;

namespace PerformanceClient.Service
{
    public class StatisticsService : IStatisticsService
    {
        private readonly HttpClient httpClient;

        public StatisticsService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Statistics> GetStatistics()
        {
            Console.WriteLine("StatisticsService::GetStatistics");

            Console.WriteLine("GETing statistics");
            var result = await httpClient.GetAsync("statistics");
            result.EnsureSuccessStatusCode();

            var json = await result.Content.ReadAsStringAsync();
            Console.WriteLine("Server replied");
            var statistics = JsonConvert.DeserializeObject<Statistics>(json);

            Console.WriteLine($"Fetched statistics");

            return statistics;
        }

        public async Task<Statistics> GetStatisticsByBoss(int bossId)
        {
            Console.WriteLine("StatisticsService::GetStatisticsByBoss");

            Console.WriteLine($"GETing bosses/{bossId}/statistics");
            var result = await httpClient.GetAsync($"bosses/{bossId}/statistics");
            result.EnsureSuccessStatusCode();

            var json = await result.Content.ReadAsStringAsync();
            Console.WriteLine("Server replied");
            var statistics = JsonConvert.DeserializeObject<Statistics>(json);

            Console.WriteLine($"Fetched statistics");

            return statistics;
        }
    }
}