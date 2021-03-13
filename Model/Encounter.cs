using System.Collections.Generic;

namespace PerformanceClient.Model
{
    public class Encounter
    {
        public Boss Boss { get; set; }
        public List<Player> Players { get; set; }
    }
}