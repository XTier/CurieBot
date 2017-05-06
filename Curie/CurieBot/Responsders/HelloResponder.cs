using System.Text;
using MargieBot;

namespace CurieBot.Responsders
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
            var builder = new StringBuilder();
            builder.Append("Hello ").Append(context.Message.User.FormattedUserID).Append("!");

            return new BotMessage { Text = builder.ToString() };
        }
    }
}