using System;
using System.Diagnostics;

namespace Data.Hid.Core
{
    [DebuggerDisplay("#{" + nameof(Output) + "}")]
    public class Measurement
    {
        public bool Success { get; }
        public int? Co2Level { get; }
        public double? Temperature { get; }
        public DateTimeOffset Time { get; set; }

        public string Output => Success ? $"{Time:T}: CO2={Co2Level}, t={Temperature:F1}" : $"{Time:T}: --";

        private Measurement(bool success, int? co2Level, double? temperature)
        {
            Success = success;
            Co2Level = co2Level;
            Temperature = temperature;
            Time = DateTimeOffset.Now;
        }

        public static Measurement WithCo2(int co2Level) => new Measurement(true, co2Level, null);

        public static Measurement WithTemperature(double temperature) => new Measurement(true, null, temperature);

        public static Measurement Failed() => new Measurement(false, null, null);
    }
}