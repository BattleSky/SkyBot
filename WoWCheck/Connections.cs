using System;
using System.IO;
using DSharpPlus;

namespace WoWCheck
{
    class Connections
    {
        private string discordKeyPath = @"Discord.tkey";
        private string discordToken;

        public  DiscordClient CreateClient()
        {
            try
            {
                using (StreamReader sr = new StreamReader(discordKeyPath))
                {
                    discordToken = sr.ReadToEnd();
                    Console.WriteLine(sr.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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


