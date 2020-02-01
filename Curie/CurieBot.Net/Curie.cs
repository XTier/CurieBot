using System;
using System.Linq;
using CurieBot.Net.Interfaces;
using CurieBot.Net.Settings;
using Data.Core;
using Data.Core.Interfaces;
using MargieBot;
using Serilog;

namespace CurieBot.Net
{
    // TODO AV: refactor
    public class Curie
    {
        private IDataProvider _dataUpdateProvider;
        private IDataProvider _dataNotifyProvider;
        private readonly IDataClient _dataClient;
        private readonly Bot _bot;

        private readonly SlackSettings _slackSettings;
        private readonly DataProviderSettings _dataProviderSettings;
        private readonly IDataProviderFactory _dataProviderFactory;

        public Curie(IDataProviderFactory dataProviderFactory, IDataClient dataClient, Bot bot, SlackSettings slackSettings, DataProviderSettings dataProviderSettings)
        {
            _dataProviderFactory = dataProviderFactory ?? throw new ArgumentNullException(nameof(dataProviderFactory));
            _dataClient = dataClient ?? throw new ArgumentNullException(nameof(dataClient));
            _bot = bot ?? throw new ArgumentNullException(nameof(bot));
            _slackSettings = slackSettings ?? throw new ArgumentNullException(nameof(slackSettings));
            _dataProviderSettings = dataProviderSettings ?? throw new ArgumentNullException(nameof(dataProviderSettings));
        }

        public void Init()
        {
            Connect();
            InitProviders();
        }

        public void Stop()
        {
            _dataUpdateProvider.Stop();
            _dataNotifyProvider.Stop();
            _bot.Disconnect();
        }

        private void InitProviders()
        {
            // data context
            var dataContext = new DataContext();
            var providerType = _dataProviderSettings.DataProviderType;

            // provider to send data to a database
            _dataUpdateProvider = _dataProviderFactory.CreateDataProvider(providerType);
            _dataUpdateProvider.NewReadout += val => _dataClient.SendData(val);
            _dataUpdateProvider.NewReadout += val => dataContext.UpdateData(val);
            _dataUpdateProvider.Init();

            // provider to notify Slack users
            _dataNotifyProvider = new Notifier(dataContext, _slackSettings);
            _dataNotifyProvider.NewReadout += msg => SendMessage(msg?.ToNiceString());
            _dataNotifyProvider.Init();
        }

        private void Connect()
        {
            try
            {
                _bot.Connect(_slackSettings.SlackBotApiToken).Wait();

                Log.Information("Bot has connected to Slack. Chats:");
                foreach (var chat in _bot.ConnectedChannels)
                {
                    Log.Information("--" + chat.Name + " " + chat.ID);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Unexpected error during bot connection. {ex}");
            }
        }

        private async void SendMessage(string message)
        {
            Log.Debug($"Notifying with '{message}'...");
            try
            {
                var slackMessage = new BotMessage
                {
                    Text = message,
                    ChatHub = _bot.ConnectedChannels.First(c => c.ID == _slackSettings.ChatId)
                };

                await _bot.Say(slackMessage);
            }
            catch (Exception ex)
            {
                Log.Error($"Unexpected error during bot notification. {ex}");
            }
        }
    }
}