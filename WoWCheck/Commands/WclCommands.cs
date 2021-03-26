using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using WoWCheck.WarcraftLogs;

namespace WoWCheck.Commands
{
    public class WclCommands : BaseCommandModule
    {

        [Command("logs")]
        [Description("Актуальная информация об игроке: статистика логов с WarcraftLogs.\n Синтаксис: `-logs метрика имя сервер `, где `метрика` это `dps` или `hps` - статистика урона и исцеления соответственно.\nДля Гордунни можно не указывать сервер.\nНапример: `-logs dps Адэльвиль Гордунни`")]
        public async Task LogsCommand(CommandContext ctx, string metric, string name)
        {
            var warcraftLogsModule = new PersonalLogsModule();
            var embed = warcraftLogsModule.PersonalLogsRequest(name, metric, new []{"гордунни"});
            await ctx.RespondAsync(embed: embed.Result);
        }
        [Command("logs")]
        public async Task LogsCommand(CommandContext ctx, string metric, string name, params string[] serverName)
        {
            var warcraftLogsModule = new PersonalLogsModule();
            var embed = warcraftLogsModule.PersonalLogsRequest(name, metric, serverName);
            await ctx.RespondAsync(embed: embed.Result);
        }
    }
}
