using System;
using System.Collections.Generic;
using System.Linq;
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

        [Inject]
        private IBossService bossService { get; set; }

        [Inject]
        private IMeasureService measureService { get; set; }

        private List<Player> players;
        private Statistics statistics;
        private List<Boss> bosses;
        private List<Measure> measures;

        public string BossId { get; set; }
        public MeasureType MeasureType {get;set;}

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine("OnInitializedAsync");
            bosses = await bossService.GetBosses();

            players = await playerService.GetPlayers();
            statistics = await statisticsService.GetStatistics();
            measures = await measureService.GetMeasures();
        }

        public PlayerStatistics GetStatistics(int playerId)
        {
            foreach (var data in statistics.Data)
            {
                if (data.Player.Id == playerId)
                {
                    Console.WriteLine($"Found data for player {data.Player.Name}");
                    return data;
                }
            }

            Console.WriteLine($"Unable to find statistics for player {playerId}");
            return new PlayerStatistics();
        }

        public List<String> GetHeaders()
        {
            return measures.Where(m=>m.Type==MeasureType.BASIC).Select(m=>m.Name).ToList();
        }

        public async void BossChanged(ChangeEventArgs e)
        {
            BossId = e.Value.ToString();
            Console.WriteLine($"Boss changed to {BossId}");
            if (BossId != null && BossId != "-1")
            {
                statistics = await statisticsService.GetStatisticsByBoss(int.Parse(BossId));
            }
            else
            {
                statistics = await statisticsService.GetStatistics();
            }
            StateHasChanged();
        }
    }
}