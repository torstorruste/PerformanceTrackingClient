using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using PerformanceClient.Model;

namespace PerformanceClient.Service
{
    public class BossService : IBossService
    {
        private readonly HttpClient httpClient;

        public BossService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Boss>> GetBosses()
        {
            Console.WriteLine("BossService::GetBosses");

            Console.WriteLine("GETing bosses");
            var result = await httpClient.GetAsync("bosses");
            result.EnsureSuccessStatusCode();

            var json = await result.Content.ReadAsStringAsync();
            var bosses = JsonConvert.DeserializeObject<List<Boss>>(json);

            Console.WriteLine($"Fetched {bosses.Count} bosses");

            foreach(var boss in bosses) {
                Console.WriteLine($"{boss.Id} - {boss.Name}");
            }

            return bosses;
        }
    }
}