namespace PerformanceClient.Model
{
    public class Measure
    {
        public string Name {get;set;}

        public MeasureType Type {get;set;}

        public string Class {get;set;}
        
        public int? BossId {get;set;}
    }
}