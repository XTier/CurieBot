using CsvHelper.Configuration;
using Data.Core.Entities;

namespace Data.Csv.Mappings
{
    public sealed class Co2ReadingMap : ClassMap<Co2Reading>
    {
        public Co2ReadingMap()
        {
            Map(m => m.Time).Name("Time");
            Map(m => m.Co2Level).Name("Co2(PPM)");
            Map(m => m.Temperature).Name("Temp");
        }
    }
}