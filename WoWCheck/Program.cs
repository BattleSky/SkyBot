using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using WoWCheck.Commands;
using WoWCheck.Converters;

namespace WoWCheck
{
    internal class Program
    {
        private static void Main()
        {
            MainTask().GetAwaiter().GetResult();
        }

        private static async Task MainTask()
        {
            var commands = Connections.Connections.Discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "-" }
            });
            commands.RegisterCommands<RioCommands>();
            commands.RegisterCommands<TextCommands>();
            commands.RegisterCommands<FunCommands>();
            commands.RegisterCommands<WclCommands>();
            commands.SetHelpFormatter<HelpFormatter>();

            await Connections.Connections.Discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}