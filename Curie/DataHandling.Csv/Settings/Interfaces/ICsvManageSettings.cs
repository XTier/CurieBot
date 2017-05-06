using System;

namespace DataHandling.Csv.Settings
{
    public interface ICsvManageSettings
    {
        TimeSpan UpdatePeriod { get; set; }
        TimeSpan DataBreakTime { get; set; }
        string ZgRootFolder { get; set; }
    }
}