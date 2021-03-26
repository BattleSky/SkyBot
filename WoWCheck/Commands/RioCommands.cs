using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using WoWCheck.RaiderIO;

namespace WoWCheck.Commands
{
    public class RioCommands : BaseCommandModule
    {
        [Command("affix")]
        [Description("Список модификаторов эпохальных+ подземелий на этой неделе")]
        public async Task AffixCommand(CommandContext ctx)
        {
            var affixesModule = new AffixesModule();
            var embed = affixesModule.AffixRequest();
            await ctx.RespondAsync(embed: embed.Result);
        }

        [Command("rio")]
        [Description("Актуальная информация об игроке: рейтинг Raider.IO и лучшие закрытые подземелья.\n Синтаксис: `-rio имя сервер`\nДля Гордунни можно не указывать сервер.\nНапример: `-rio Адэльвиль Гордунни`")]
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
