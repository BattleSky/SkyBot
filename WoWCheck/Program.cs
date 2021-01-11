using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using WoWCheck.RaiderIO;

namespace WoWCheck
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MainTask(args).GetAwaiter().GetResult();
        }


        private static async Task MainTask(string[] args)
        {
            var discord = new Connections().CreateClient();
            
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "-" }
            });
            commands.RegisterCommands<RioCommands>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}