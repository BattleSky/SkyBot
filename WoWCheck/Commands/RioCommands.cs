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
            var embed = affixesModule.AffixRequest(ctx.User.AvatarUrl);
            await ctx.RespondAsync(embed: embed.Result);
        }

        [Command("rio")]
        public async Task RioCommand(CommandContext ctx, string name)
        {
            var mythicPlusModule = new MythicPlusModule();
            var embed = mythicPlusModule.MythicPlusRequest(name, new []{"гордунни"});
            await ctx.RespondAsync(embed: embed.Result);
        }
        [Command("rio")]
        public async Task RioCommand(CommandContext ctx, string name, params string[] serverName)
        {
            var mythicPlusModule = new MythicPlusModule();
            var embed = mythicPlusModule.MythicPlusRequest(name, serverName);
            await ctx.RespondAsync(embed: embed.Result);
        }
    }
}
