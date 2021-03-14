namespace PerformanceClient.Model
{
    public class Ranking
    {
        public Boss Boss { get; set; }
        public Player Player { get; set; }
        public RankingType rankingType { get; set; }
        public int rank { get; set; }
    }
}