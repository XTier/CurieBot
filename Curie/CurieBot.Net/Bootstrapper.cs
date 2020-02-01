using System;
using System.Reflection;
using CurieBot.Net.Responders;
using CurieBot.Net.Settings;
using Data.Core.Interfaces;
using Data.Csv.Settings;
using Data.Hid.Core.Settings;
using MargieBot;
using Microsoft.Extensions.Configuration;
using Sink.InfluxDb;
using Sink.InfluxDb.Settings;
using Unity;
using Unity.Lifetime;

namespace CurieBot.Net
{
    public class Bootstrapper
    {
        private readonly IConfigurationRoot _configurationRoot = new ConfigurationBuilder()
            .AddXmlFile($"{Assembly.GetExecutingAssembly().Location}.config", false, true).Build();

        private readonly UnityContainer _container = new UnityContainer();

        public void Initialize()
        {
            RegisterTypes();
        }

        public T ResolveWorkflow<T>()
        {
            try
            {
                return _container.Resolve<T>();
            }
            catch (ResolutionFailedException ex)
            {
                throw new InvalidOperationException($"Failed to resolve a {typeof(T)} entity via Unity.", ex);
            }
        }

        private void RegisterTypes()
        {
            _container
                .RegisterInstance(GetSettings<DatabaseSettings>())
                .RegisterInstance(GetSettings<SlackSettings>())
                .RegisterInstance(GetSettings<CsvManageSettings>())
                .RegisterInstance(GetSettings<HidManageSettings>())
                .RegisterInstance(GetSettings<DataProviderSettings>())
                .RegisterSingleton<IResponder, HelloResponder>()
                .RegisterSingleton<IDataClient, InfluxDataClient>()
                .RegisterFactory<Bot>(InitializeBot, new SingletonLifetimeManager());
        }

        private Bot InitializeBot(IUnityContainer container)
        {
            var bot = new Bot();
            var responders = container.ResolveAll<IResponder>();
            bot.Responders.AddRange(responders);
            return bot;
        }

        private T GetSettings<T>() => _configurationRoot.GetSection(typeof(T).Name).Get<T>();
    }
}