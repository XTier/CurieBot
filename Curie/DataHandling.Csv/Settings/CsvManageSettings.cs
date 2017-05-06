using System;

namespace DataHandling.Csv.Settings
{
    public class CsvManageSettings : ICsvManageSettings
    {
        public TimeSpan UpdatePeriod { get; set; }
        public TimeSpan DataBreakTime { get; set; }
        public string ZgRootFolder { get; set; }
    }
}