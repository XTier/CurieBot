using System.Configuration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using MargieBot;

namespace CurieBot
{
    public class BotConfigurator
    {
        public static Bot Create()
        {
            var container = new WindsorContainer();
            container.Install(FromAssembly.This());

            var bot = new Bot();
            var responders = container.ResolveAll<IResponder>();
            bot.Responders.AddRange(responders);

            return bot;
        }

        public static string RetrieveApiToken()
        {
            return ConfigurationManager.AppSettings["SlackBotApiToken"];
        }
    }
}