using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using PerformanceClient.Model;

namespace PerformanceClient.Service
{
    public class RankingService : IRankingService
    {
        private readonly HttpClient httpClient;

        public RankingService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Ranking>> GetRankings()
        {
            Console.WriteLine("RankingService::GetRankings");

            Console.WriteLine("GETing rankings");
            var result = await httpClient.GetAsync("rankings");
            result.EnsureSuccessStatusCode();

            var json = await result.Content.ReadAsStringAsync();
            var rankings = JsonConvert.DeserializeObject<List<Ranking>>(json);

            Console.WriteLine($"Fetched {rankings.Count} rankings");

            return rankings;
        }
    }
}