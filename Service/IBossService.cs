using System.Collections.Generic;
using System.Threading.Tasks;
using PerformanceClient.Model;

namespace PerformanceClient.Service
{
    public interface IBossService {
        Task<List<Boss>> GetBosses();
    }
}