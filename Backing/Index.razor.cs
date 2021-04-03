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
        private Dictionary<int, PlayerStatistics> statisticsMap;
        private List<Boss> bosses;
        private List<Measure> measures;
        private List<Ranking> rankings;
        private List<String> headers;

        public string BossId { get; set; } = "-1";
        public MeasureType MeasureType { get; set; } = MeasureType.BASIC;

        public EncounterType ET { get; set; } = EncounterType.BOTH;

        public String SelectedClass = "ALLPLAYERS";

        private string currentSort = "Name";

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine("OnInitializedAsync");
            bosses = await bossService.GetBosses();

            players = await playerService.GetPlayers();
            statistics = await statisticsService.GetStatistics(ET);
            CreateStatisticsMap();

            measures = await measureService.GetMeasures();
            rankings = await rankingService.GetRankings();

            headers = DetermineHeaders();
        }

        private void CreateStatisticsMap()
        {
            statisticsMap = new Dictionary<int, PlayerStatistics>();
            foreach (var playerStat in statistics.Data)
            {
                statisticsMap[playerStat.Player.Id] = playerStat;
            }
        }

        public PlayerStatistics GetStatistics(int playerId)
        {
            if (statisticsMap.ContainsKey(playerId))
            {
                return statisticsMap[playerId];
            }

            return new PlayerStatistics();
        }

        public List<String> GetHeaders() {
            return headers;
        }

        public List<String> DetermineHeaders()
        {
            var currentPlayers = GetPlayers();
            List<PlayerStatistics> currentStatistics = currentPlayers.Select(p=>GetStatistics(p.Id)).ToList();
            return measures.Where(m => m.Type == MeasureType)
                    .Where(m => HasValue(m.Name, currentStatistics))
                    .Select(m => m.Name).ToList();
        }

        public bool HasValue(String header, List<PlayerStatistics> currentStatistics)
        {
            foreach (var statistics in currentStatistics)
            {
                if (HasValue(statistics, header)) return true;
            }

            return false;
        }

        public bool HasValue(PlayerStatistics playerStatistics, String header)
        {
            return playerStatistics != null && playerStatistics.Data != null && playerStatistics.Data.ContainsKey(header);
        }

        public int GetValue(PlayerStatistics playerStatistics, string header)
        {
            if (playerStatistics != null && playerStatistics.Data != null)
            {
                if (playerStatistics.Data.ContainsKey(header))
                {
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

        private List<Ranking> GetRankings(int playerId, RankingType rankingType)
        {
            if (BossId == null || BossId == "-1")
                return this.rankings.Where(r => r.Player.Id == playerId && r.rankingType == rankingType).ToList();
            else
                return this.rankings.Where(r => r.Player.Id == playerId && r.Boss.Id == int.Parse(BossId) && r.rankingType == rankingType).ToList();
        }

        public int? GetMinRank(Player player, RankingType rankingType)
        {
            var rankings = GetRankings(player.Id, rankingType);

            if (rankings.Count > 0)
            {
                return rankings.Min(r => r.rank);
            }
            else
            {
                return null;
            }
        }

        public double? GetAverageRank(Player player, RankingType rankingType)
        {
            var rankings = GetRankings(player.Id, rankingType);
            if (rankings.Count > 0)
            {
                return Math.Round(rankings.Average(r => r.rank), 2);
            }
            else
            {
                return null;
            }
        }

        public int? GetMaxRank(Player player, RankingType rankingType)
        {
            var rankings = GetRankings(player.Id, rankingType);

            if (rankings.Count > 0)
            {
                return rankings.Max(r => r.rank);
            }
            else
            {
                return null;
            }
        }

        public async void BossChanged(ChangeEventArgs e)
        {
            BossId = e.Value.ToString();
            Console.WriteLine($"Boss changed to {BossId}");
            if (BossId != null && BossId != "-1")
            {
                statistics = await statisticsService.GetStatisticsByBoss(ET, int.Parse(BossId));
            }
            else
            {
                statistics = await statisticsService.GetStatistics(ET);
            }
            CreateStatisticsMap();
            currentSort = "OldBoss";
            headers = DetermineHeaders();
            StateHasChanged();
        }

        public void MeasureChanged(ChangeEventArgs e)
        {
            MeasureType = (MeasureType)Enum.Parse(typeof(MeasureType), e.Value.ToString().ToUpper());

            headers = DetermineHeaders();
            StateHasChanged();
        }

        public async void EncounterTypeChanged(ChangeEventArgs e)
        {
            ET = (EncounterType)Enum.Parse(typeof(EncounterType), e.Value.ToString().ToUpper());

            if (BossId != null && BossId != "-1")
            {
                statistics = await statisticsService.GetStatisticsByBoss(ET, int.Parse(BossId));
            }
            else
            {
                statistics = await statisticsService.GetStatistics(ET);
            }

            headers = DetermineHeaders();
            CreateStatisticsMap();
            StateHasChanged();
        }

        public void PlayerClassChanged(ChangeEventArgs e)
        {
            SelectedClass = e.Value.ToString().Replace(" ", "").ToUpper();

            headers = DetermineHeaders();
            StateHasChanged();
        }

        public List<Player> GetPlayers()
        {
            if (SelectedClass == "ALLPLAYERS")
            {
                return players;
            }
            else
            {
                return players.Where(p => p.Class.ToUpper() == SelectedClass).ToList();
            }
        }

        public void Sort(String header)
        {
            if (currentSort == null || currentSort != header)
            {
                Console.WriteLine($"Sorting by {header}");

                players = players.OrderBy(p => GetValuePerEncounter(GetStatistics(p.Id), header))
                .ThenBy(p => GetValue(GetStatistics(p.Id), header)).Reverse().ToList();
            }
            else
            {
                players.Reverse();
            }
            currentSort = header;
            StateHasChanged();
        }

        public void SortByName()
        {
            if (currentSort != "Name")
            {
                players = players.OrderBy(p => p.Name).ToList();
            }
            else
            {
                players.Reverse();
            }
            currentSort = "Name";
            StateHasChanged();
        }

        public void SortByFarm()
        {
            if (currentSort != "Farm")
            {
                players = players.OrderBy(p => GetValue(GetStatistics(p.Id), "Farm")).Reverse().ToList();
            }
            else
            {
                players.Reverse();
            }
            currentSort = "Farm";
            StateHasChanged();
        }

        public void SortByProgress()
        {
            if (currentSort != "Progress")
            {
                players = players.OrderBy(p => GetValue(GetStatistics(p.Id), "Progress")).Reverse().ToList();
            }
            else
            {
                players.Reverse();
            }
            currentSort = "Progress";
            StateHasChanged();
        }

        public void SortByDPS()
        {
            if (currentSort != "DPS")
            {
                players = players.OrderBy(p => GetAverageRank(p, RankingType.DPS)).Reverse().ToList();
            }
            else
            {
                players.Reverse();
            }
            currentSort = "DPS";
            StateHasChanged();
        }

        public void SortByHPS()
        {
            if (currentSort != "HPS")
            {
                players = players.OrderBy(p => GetAverageRank(p, RankingType.HPS)).Reverse().ToList();
            }
            else
            {
                players.Reverse();
            }
            currentSort = "HPS";
            StateHasChanged();
        }
    }
}