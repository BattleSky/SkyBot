using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using WoWCheck.RaiderIO;
using WoWCheck.WarcraftLogs;

namespace WoWCheck
{
    public class WclCommands : BaseCommandModule
    {

        [Command("logs")]
        public async Task LogsCommand(CommandContext ctx, string metric, string name)
        {
            var warcraftLogsModule = new PersonalLogsModule();
            var embed = warcraftLogsModule.PersonalLogsRequest(name, metric, new []{"гордунни"});
            await ctx.RespondAsync(embed: embed.Result);
        }
        [Command("logs")]
        public async Task LogsCommand(CommandContext ctx, string metric, string name, params string[] serverName)
        {
            throw new NotImplementedException();
            var mythicPlusModule = new MythicPlusModule();
            var embed = mythicPlusModule.MythicPlusRequest(name, serverName);
            await ctx.RespondAsync(embed: embed.Result);
        }
    }
}
