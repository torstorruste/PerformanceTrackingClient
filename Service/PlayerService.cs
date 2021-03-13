using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using PerformanceClient.Model;

namespace PerformanceClient.Service
{
    public class PlayerService : IPlayerService
    {
        private readonly HttpClient httpClient;

        public PlayerService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Player>> GetPlayers()
        {
            Console.WriteLine("PlayerService::GetPlayers");

            Console.WriteLine("GETing players");
            var result = await httpClient.GetAsync("players");
            result.EnsureSuccessStatusCode();

            var json = await result.Content.ReadAsStringAsync();
            var players = JsonConvert.DeserializeObject<List<Player>>(json);

            Console.WriteLine($"Fetched {players.Count} players");

            return players;
        }
    }
}