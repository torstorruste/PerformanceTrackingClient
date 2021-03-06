using System.Collections.Generic;
using System.Threading.Tasks;
using PerformanceClient.Model;

namespace PerformanceClient.Service
{
    public interface IRankingService
    {
        Task<List<Ranking>> GetRankings();
    }
}