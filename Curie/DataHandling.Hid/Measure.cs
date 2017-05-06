namespace DataHandling.Hid
{
    internal class Measure
    {
        internal int? Co2Level { get; set; }
        internal double? Temperature { get; set; }

        internal void Apply(Measure measure)
        {
            Co2Level = measure.Co2Level ?? Co2Level;
            Temperature = measure.Temperature ?? Temperature;
        }
    }
}