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

        public async Task<Statistics> GetStatistics(EncounterType encounterType)
        {
            Console.WriteLine("StatisticsService::GetStatistics");

            Console.WriteLine($"GETing statistics?encounterType={encounterType}");
            var result = await httpClient.GetAsync($"statistics?encounterType={encounterType}");
            result.EnsureSuccessStatusCode();

            var json = await result.Content.ReadAsStringAsync();
            Console.WriteLine("Server replied");
            var statistics = JsonConvert.DeserializeObject<Statistics>(json);

            Console.WriteLine($"Fetched statistics");

            return statistics;
        }

        public async Task<Statistics> GetStatisticsByBoss(EncounterType encounterType, int bossId)
        {
            Console.WriteLine("StatisticsService::GetStatisticsByBoss");

            Console.WriteLine($"GETing bosses/{bossId}/statistics?encounterType={encounterType}");
            var result = await httpClient.GetAsync($"bosses/{bossId}/statistics?encounterType={encounterType}");
            result.EnsureSuccessStatusCode();

            var json = await result.Content.ReadAsStringAsync();
            Console.WriteLine("Server replied");
            var statistics = JsonConvert.DeserializeObject<Statistics>(json);

            Console.WriteLine($"Fetched statistics");

            return statistics;
        }
    }
}