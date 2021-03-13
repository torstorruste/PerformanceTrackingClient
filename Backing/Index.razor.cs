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
        public MeasureType MeasureType { get; set; } = MeasureType.BASIC;

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
            return measures.Where(m => m.Type == MeasureType).Select(m => m.Name).Where(h => HasValue(h)).ToList();
        }

        public bool HasValue(String header)
        {
            foreach (var data in statistics.Data)
            {
                if(data.Data.ContainsKey(header)) return true;
            }
            return false;
        }

        public bool HasValue(PlayerStatistics playerStatistics, String header)
        {
            return playerStatistics.Data.ContainsKey(header);
        }

        public int GetValue(PlayerStatistics playerStatistics, string header) {
            return playerStatistics.Data[header];
        }

        public double GetValuePerEncounter(PlayerStatistics playerStatistics, string header) {
            int numEncounters = playerStatistics.Data["Farm"] + playerStatistics.Data["Progress"];
            int value = playerStatistics.Data[header];

            double result = ((double)value)/numEncounters;

            return Math.Round(result, 2);
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

        public async void MeasureChanged(ChangeEventArgs e)
        {
            MeasureType = (MeasureType)Enum.Parse(typeof(MeasureType), e.Value.ToString().ToUpper());

            StateHasChanged();
        }
    }
}