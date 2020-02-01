using System;

namespace CurieBot.Net.Settings
{
    public class SlackSettings
    {
        public string ChatId { get; set; }
        public TimeSpan NotifyPeriod { get; set; }
        public TimeSpan DelayOnErrorTime { get; set; }
        public int ErrorAttemptsThreshold { get; set; }
        public string SlackBotApiToken { get; set; }
    }
}