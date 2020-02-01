using System.Text;
using MargieBot;

namespace CurieBot.Net.Responders
{
    public class HelloResponder : IResponder
    {
        public bool CanRespond(ResponseContext context)
        {
            return context.Message.MentionsBot
                   && !context.BotHasResponded
                   && context.Message.Text.ToLower().Contains("hello");
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            var builder = new StringBuilder().Append("Hello ").Append(context.Message.User.FormattedUserID).Append("!");

            return new BotMessage {Text = builder.ToString()};
        }
    }
}