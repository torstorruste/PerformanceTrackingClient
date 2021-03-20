using Newtonsoft.Json;

namespace PerformanceClient.Model
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonProperty("playerClass")]
        public string Class { get; set; }

        public string CssClass() {
            return Class.Substring(0, 1).ToUpper()+Class.Substring(1).ToLower();
        }
    }
}