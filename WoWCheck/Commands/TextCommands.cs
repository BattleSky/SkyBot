using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;


namespace WoWCheck.Commands
{
    class TextCommands : BaseCommandModule
    {
        [Command("hi")]
        [Description("Поприветствуй бота!")]
        public async Task GreetCommand(CommandContext ctx)
        {
            var author = ctx.User.Mention;
            await ctx.RespondAsync("Привет, " + author);
        }
        //[Command("affix")]
        //[Description("Показать расписание модификаторов эпохальных подземелий")]
        //public async Task AffixCommand(CommandContext ctx, string all)
        //{
        //    var affixesModule = new ();
        //    var embed = affixesModule.AffixRequest(ctx.User.AvatarUrl);
        //    await ctx.RespondAsync(embed: embed.Result);
        //}




        #region Фигня всякая

        // Не работает? 
        // Не получает список имен в ctx.Channel.Users
        //[Command("hi")]
        //public async Task GreetCommandMentionsOther(CommandContext ctx, string name)
        //{
        //    if (ctx.Channel.Id != 241874656318062593) return;
        //    var mention = "";
        //    foreach (var user in ctx.Channel.Users)
        //    {
        //        Console.WriteLine("user:" + user.DisplayName + "\n");

        //        if (user.DisplayName != name || user.Nickname != name) continue;
        //        mention = user.Mention;
        //        break;
        //    }

        //    if (mention == "")
        //        await ctx.RespondAsync("Не понял, кому передать привет");
        //    else
        //        await ctx.RespondAsync("Тебе привет, " + mention);
        //}

        #endregion

    }
}
