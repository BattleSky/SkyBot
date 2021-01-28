﻿using System;
using System.IO;
using DSharpPlus;
using Microsoft.Extensions.Logging;


namespace WoWCheck
{
    class Connections
    {
        #if DEBUG 
        private readonly string discordKeyPath = @"Connections/DiscordTEST.tkey";
        #else
        private readonly string discordKeyPath = @"Discord.tkey";
        #endif

        public static DiscordClient Discord = new Connections().CreateClient();

        public  DiscordClient CreateClient()
        {
            string discordToken;
            try
            {
                using (StreamReader sr = new StreamReader(discordKeyPath))
                    discordToken = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine("Discord Read Key:" + e.Message);
                throw;
            }
            var discord = new DiscordClient(new DiscordConfiguration
            {
                Token = discordToken,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug
            });
            return discord;
        }
    }
}

