using DSharpPlus;

namespace WoWCheck
{
    class Connections
    {
        private string token = "Nzk2NDcyMTU5MTM1NzI3NjQ2.X_YagA.3sf9YuQM08pTixG2YvfUlFPa-e0";
        public  DiscordClient CreateClient()
        {
            var discord = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });
            return discord;
        }
    }
}
