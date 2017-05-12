using System;

namespace CurieBot.Settings.Interfaces
{
    public interface ISlackSettings
    {
        string ChatId { get; }
        TimeSpan NotifyPeriod { get; }
        TimeSpan DelayOnErrorTime { get; }
        int ErrorAttemptsThreshold { get; }
    }
}