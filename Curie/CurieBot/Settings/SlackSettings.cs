using System;
using CurieBot.Settings.Interfaces;

namespace CurieBot.Settings
{
    public class SlackSettings : ISlackSettings
    {
        public string ChatId { get; set; }
        public TimeSpan NotifyPeriod { get; set; }
        public TimeSpan DelayOnErrorTime { get; set; }
    }
}