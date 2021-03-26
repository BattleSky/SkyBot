using System;
using System.IO;
using DSharpPlus;
using Microsoft.Extensions.Logging;

namespace WoWCheck.Connections
{
    internal class Connections
    {
        #if DEBUG 
        private readonly string discordKeyPath = @"Connections/DiscordTEST.tkey";
        #else
        private readonly string discordKeyPath = @"Connections/Discord.tkey";
        #endif

        public static DiscordClient Discord = new Connections().CreateClient();

        public DiscordClient CreateClient()
        {
            string discordToken;
            try
            {
                using var sr = new StreamReader(discordKeyPath);
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
#if DEBUG
                MinimumLogLevel = LogLevel.Debug
#else
                MinimumLogLevel = LogLevel.Warning
#endif
            });
            return discord;
        }
    }
}


