using System.Collections.Generic;

namespace PerformanceClient.Model
{
    public class PlayerStatistics
    {
        public Player Player { get; set; }

        public Dictionary<string, int> Data { get; set; }
    }
}