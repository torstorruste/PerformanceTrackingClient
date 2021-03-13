using System.Collections.Generic;
using System.Threading.Tasks;
using PerformanceClient.Model;

namespace PerformanceClient.Service
{
    public interface IMeasureService {
        Task<List<Measure>> GetMeasures();
    }
}