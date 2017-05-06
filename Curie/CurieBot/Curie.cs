using System.Linq;
using DataClient.InfluxDb;
using DataHandling.Core;
using DataHandling.Core.Interfaces;
using MargieBot;
using Serilog;
using SimpleConfig;

namespace CurieBot
{
    public class Curie
    {
        private IDataProvider _dataUpdateProvider;
        private IDataProvider _dataNotifyProvider;
        private IDataClient _dataClient;
        private readonly Bot _bot;
        private readonly string _apiToken;
        private string _chatId;

        public Curie()
        {
            _bot = BotConfigurator.Create();
            _apiToken = BotConfigurator.RetrieveApiToken();
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
            // settings
            var settings = Configuration.Load<Settings.Settings>();
            _chatId = settings.SlackSettings.ChatId;

            // data context
            var dataContext = new DataContext();

            // database client
            _dataClient = new InfluxDataClient(settings.DatabaseSettings);

            // pre-initialization
            var dataProviderFactory = new DataProviderFactory(settings.CsvManageSettings, settings.HidManageSettings);
            var providerType = settings.DataProviderType;

            // provider to send data to a database
            _dataUpdateProvider = dataProviderFactory.CreateDataProvider(providerType);
            _dataUpdateProvider.NewReadout += val => _dataClient.SendData(val);
            _dataUpdateProvider.NewReadout += val => dataContext.UpdateData(val);
            _dataUpdateProvider.Init();

            // provider to notify Slack users
            _dataNotifyProvider = new Notifier(dataContext, settings.SlackSettings);
            _dataNotifyProvider.NewReadout += msg => SendMessage(msg.ToNiceString());
            _dataNotifyProvider.Init();
        }

        private void Connect()
        {
            _bot.Connect(_apiToken).Wait();

            Log.Information("Bot has connected to Slack. Chats:");
            foreach (var chat in _bot.ConnectedChannels)
            {
                Log.Information("--" + chat.Name + " " + chat.ID);
            }
        }

        private async void SendMessage(string message)
        {
            Log.Debug($"Notifying with '{message}'...");
            var slackMessage = new BotMessage
            {
                Text = message,
                ChatHub = _bot.ConnectedChannels?.First(c => c.ID == _chatId)
            };

            await _bot.Say(slackMessage);
        }
    }
}