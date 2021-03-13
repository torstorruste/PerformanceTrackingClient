using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PerformanceClient.Model;

namespace PerformanceClient.Service
{
    public class MeasureService : IMeasureService
    {
        private readonly HttpClient httpClient;

        public MeasureService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Measure>> GetMeasures()
        {
            Console.WriteLine("MeasureService::GetMeasures");

            Console.WriteLine("GETing measures");
            var result = await httpClient.GetAsync("measures");
            result.EnsureSuccessStatusCode();

            var json = await result.Content.ReadAsStringAsync();
            var measures = JsonConvert.DeserializeObject<List<Measure>>(json);

            Console.WriteLine($"Fetched {measures.Count} measures");

            return measures;
        }
    }
}