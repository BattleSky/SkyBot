using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using WoWCheck.RaiderIO;

namespace WoWCheck
{
    public class RioCommands : BaseCommandModule
    {
        [Command("affix")]
        public async Task AffixCommand(CommandContext ctx)
        {
            var affixesModule = new AffixesModule();
            if (ctx.Channel.Id != 241874656318062593) return;
            var embed = affixesModule.AffixRequest(ctx.User.AvatarUrl);
            await ctx.RespondAsync(embed: embed.Result);
        }

    }
}
