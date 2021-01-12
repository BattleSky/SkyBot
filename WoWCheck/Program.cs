﻿using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using WoWCheck.Commands;
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
            
            var commands = Connections.Discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "-" }
            });
            commands.RegisterCommands<RioCommands>();
            commands.RegisterCommands<TextCommands>();

            await Connections.Discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}