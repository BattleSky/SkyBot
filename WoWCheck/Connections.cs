using System;
using System.IO;
using DSharpPlus;
using Microsoft.Extensions.Logging;


namespace WoWCheck
{
    class Connections
    {
        private string discordKeyPath = @"DiscordTEST.tkey";
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


