using System;
using System.Threading.Tasks;
using DSharpPlus;

namespace WoWCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            MainTask(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainTask(string[] args)
        {
            var discordConnect = new DiscordConnect();
            var discord = discordConnect.CreateClient();

            discord.MessageCreated += async e =>
            {
                var message = e.Message.Content;
                if (message.StartsWith("&"))
                {
                    await e.Message.RespondAsync("Hello, " +e.Author.Username);
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
