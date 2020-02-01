using System;

namespace Data.Core.Entities
{
    public class Co2Reading
    {
        public Co2Reading()
        {
            IsSuccess = true;
        }

        public Co2Reading(DateTime time, int co2Level, double temperature, bool isSuccess = true, string error = "")
        {
            Time = time;
            Co2Level = co2Level;
            Temperature = temperature;
            IsSuccess = isSuccess;
            Error = error;
        }

        public DateTime Time { get; set; }
        public int Co2Level { get; set; }
        public double Temperature { get; set; }

        public bool IsSuccess { get; set; }
        public string Error { get; set; }

        public string ToNiceString()
        {
            var result = IsSuccess ? $"CO2 level: {Co2Level} ppm, temperature: {Sign(Temperature)}{Temperature:##.#} °C" : $"{Error}";
            return result;
        }

        public override string ToString()
        {
            var result = IsSuccess ? $"{Time.TimeOfDay} {Co2Level} {Temperature}" : $"CO2 readings: {Error}";
            return result;
        }

        private static string Sign(double temperature)
        {
            return temperature > 0
                ? "+"
                : temperature < 0 ? "-" : string.Empty;
        }

        public static Co2Reading Create(DateTime time, int co2Level, double temperature)
        {
            return new Co2Reading(time, co2Level, temperature);
        }

        public static Co2Reading CreateError(string error)
        {
            return new Co2Reading(DateTime.Now, 0, 0, false, error);          // TODO UTC?
        }
    }
}
