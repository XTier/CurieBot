using System;

namespace Data.Csv.Settings
{
    public class CsvManageSettings
    {
        public TimeSpan UpdatePeriod { get; set; }
        public TimeSpan DataBreakTime { get; set; }
        public string ZgRootFolder { get; set; }
    }
}