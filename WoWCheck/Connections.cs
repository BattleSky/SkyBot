using System;
using System.IO;
using DSharpPlus;

namespace WoWCheck
{
    class Connections
    {
        private string discordKeyPath = @"Discord.tkey";
        
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
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });
            return discord;
        }
    }
}


