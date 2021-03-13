using System.Collections.Generic;
using System.Threading.Tasks;
using PerformanceClient.Model;

namespace PerformanceClient.Service
{
    public interface IStatisticsService {
        Task<Statistics> GetStatistics();

        Task<Statistics> GetStatisticsByBoss(int bossId);
    }
}