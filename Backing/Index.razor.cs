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

        [Inject]
        private IRankingService rankingService { get; set; }

        private List<Player> players;
        private Statistics statistics;
        private List<Boss> bosses;
        private List<Measure> measures;
        private List<Ranking> rankings;

        public string BossId { get; set; } = "-1";
        public MeasureType MeasureType { get; set; } = MeasureType.BASIC;

        private string currentSort = "Name";

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine("OnInitializedAsync");
            bosses = await bossService.GetBosses();

            players = await playerService.GetPlayers();
            statistics = await statisticsService.GetStatistics();
            measures = await measureService.GetMeasures();
            rankings = await rankingService.GetRankings();
        }

        public PlayerStatistics GetStatistics(int playerId)
        {
            foreach (var data in statistics.Data)
            {
                if (data.Player.Id == playerId)
                {
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
                if (data.Data.ContainsKey(header)) return true;
            }
            return false;
        }

        public bool HasValue(PlayerStatistics playerStatistics, String header)
        {
            return playerStatistics.Data.ContainsKey(header);
        }

        public int GetValue(PlayerStatistics playerStatistics, string header)
        {
            if(playerStatistics!=null && playerStatistics.Data!=null) {
                if(playerStatistics.Data.ContainsKey(header)) {
                    return playerStatistics.Data[header];
                } 
            }

            return 0;
        }

        public double GetValuePerEncounter(PlayerStatistics playerStatistics, string header)
        {
            int numEncounters = GetValue(playerStatistics, "Farm") + GetValue(playerStatistics, "Progress");
            int value = GetValue(playerStatistics, header);

            double result = ((double)value) / numEncounters;

            return Math.Round(result, 2);
        }

        private List<Ranking> GetRankings(int playerId, RankingType rankingType) {
            if(BossId==null || BossId=="-1")
                return this.rankings.Where(r=>r.Player.Id==playerId && r.rankingType==rankingType).ToList();
            else
                return this.rankings.Where(r=>r.Player.Id==playerId && r.Boss.Id==int.Parse(BossId) && r.rankingType==rankingType).ToList();
        }

        public int? GetMinRank(Player player, RankingType rankingType) {
            var rankings = GetRankings(player.Id, rankingType);

            if(rankings.Count>0) {
                return rankings.Min(r=>r.rank);
            } else {
                return null;
            }
        }

        public double? GetAverageRank(Player player, RankingType  rankingType) {
            var rankings = GetRankings(player.Id, rankingType);
            if(rankings.Count>0) {
                return Math.Round(rankings.Average(r=>r.rank), 2);
            } else {
                return null;
            }
        }

        public int? GetMaxRank(Player player, RankingType rankingType) {
            var rankings = GetRankings(player.Id, rankingType);

            if(rankings.Count>0) {
                return rankings.Max(r=>r.rank);
            } else {
                return null;
            }
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
            currentSort = "OldBoss";
            StateHasChanged();
        }

        public void MeasureChanged(ChangeEventArgs e)
        {
            MeasureType = (MeasureType)Enum.Parse(typeof(MeasureType), e.Value.ToString().ToUpper());

            StateHasChanged();
        }

        public void Sort(String header) {
            if(currentSort==null || currentSort!= header) {
                Console.WriteLine($"Sorting by {header}");

                players = players.OrderBy(p=>GetValuePerEncounter(GetStatistics(p.Id), header))
                .ThenBy(p=>GetValue(GetStatistics(p.Id), header)).Reverse().ToList();
            } else {
                players.Reverse();
            }
            currentSort = header;
            StateHasChanged();
        }

        public void SortByName() {
            if(currentSort!="Name") {
                players = players.OrderBy(p=>p.Name).ToList();
            } else {
                players.Reverse();
            }
            currentSort = "Name";
            StateHasChanged();
        }

        public void SortByFarm() {
            if(currentSort!="Farm") {
                players = players.OrderBy(p=>GetValue(GetStatistics(p.Id), "Farm")).Reverse().ToList();
            } else {
                players.Reverse();
            }
            currentSort = "Farm";
            StateHasChanged();
        }

        public void SortByProgress() {
            if(currentSort!="Progress") {
                players = players.OrderBy(p=>GetValue(GetStatistics(p.Id), "Progress")).Reverse().ToList();
            } else {
                players.Reverse();
            }
            currentSort = "Progress";
            StateHasChanged();
        }

        public void SortByDPS() {
            if(currentSort!="DPS") {
                players = players.OrderBy(p=>GetAverageRank(p, RankingType.DPS)).Reverse().ToList();
            } else {
                players.Reverse();
            }
            currentSort = "DPS";
            StateHasChanged();
        }

        public void SortByHPS() {
            if(currentSort!="HPS") {
                players = players.OrderBy(p=>GetAverageRank(p, RankingType.HPS)).Reverse().ToList();
            } else {
                players.Reverse();
            }
            currentSort = "HPS";
            StateHasChanged();
        }
    }
}