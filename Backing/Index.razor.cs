using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PerformanceClient.Model;
using PerformanceClient.Service;

namespace PerformanceClient.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private IPlayerService playerService { get; set; }

        [Inject]
        private IStatisticsService statisticsService { get; set; }

        private List<Player> players;
        private Statistics statistics;

        protected override async Task OnInitializedAsync()
        {
            players = await playerService.GetPlayers();
            statistics = await statisticsService.GetStatistics();
        }

        public PlayerStatistics GetStatistics(int playerId)
        {
            foreach(var data in statistics.Data) {
                if(data.Player.Id==playerId) {
                    Console.WriteLine($"Found data for player {data.Player.Name}");
                    return data;
                }
            }

            Console.WriteLine($"Unable to find statistics for player {playerId}");
            return new PlayerStatistics();
        }
    }
}