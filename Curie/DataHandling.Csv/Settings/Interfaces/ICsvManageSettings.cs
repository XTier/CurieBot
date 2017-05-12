using System;

namespace DataHandling.Csv.Settings
{
    public interface ICsvManageSettings
    {
        TimeSpan UpdatePeriod { get; }
        TimeSpan DataBreakTime { get; }
        string ZgRootFolder { get; }
    }
}