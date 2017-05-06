using System;

namespace CurieBot.Settings.Interfaces
{
    public interface ISlackSettings
    {
        string ChatId { get; set; }
        TimeSpan NotifyPeriod { get; set; }
        TimeSpan DelayOnErrorTime { get; set; }

    }
}