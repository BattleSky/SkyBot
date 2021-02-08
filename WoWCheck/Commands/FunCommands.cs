using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using WoWCheck.Games;

namespace WoWCheck.Commands
{
    class FunCommands : BaseCommandModule
    {
        [Command("смотринаменя")]
        [Description("Заставляет бота посмотреть на вас.")]
        public async Task StatusLookingAt(CommandContext ctx)
        {
            var activity = new DiscordActivity(" на " + ctx.Member.DisplayName, ActivityType.Watching);
            // При шардировании надо будет изменить эмодзю
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(Connections.Discord, ":angry_eyes:"));
            await Connections.Discord.UpdateStatusAsync(activity);
        }

        [Command("heal")]
        [Description("Запустить исцеляй-игру или исцелить в ней.")]
        public async Task HealGameCommand(CommandContext ctx)
        {
            if (!HealGame.IsGameActive)
                await ctx.RespondAsync(embed: HealGame.InitializeHealGame());
            else 
                await ctx.RespondAsync(embed: HealGame.ResultOfOneRound(ctx));
        }
    }
}
