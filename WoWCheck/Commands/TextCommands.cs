using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;


namespace WoWCheck.Commands
{
    internal class TextCommands : BaseCommandModule
    {
        [Command("hi")]
        [Description("Поприветствуй бота!")]
        public async Task GreetCommand(CommandContext ctx)
        {
            var author = ctx.User.Mention;
            await ctx.RespondAsync("Привет, " + author);
        }
    }
}
