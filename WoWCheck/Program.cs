using System;
using System.Threading.Tasks;
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
            var affixesModule = new AffixesModule();

            discord.MessageCreated += async e =>
            {
                var message = e.Message.Content;
                if (e.Channel.Id != 241874656318062593) return;
                if (message.StartsWith("-")) // TODO: Есть возможность реализовать это по-другому, в доке описание
                {
                    if (message.Contains("affix")) // TODO: Есть возможность реализовать это по-другому, в доке описание
                       affixesModule.AffixRequest(discord, e);
                    
                    //await e.Message.RespondAsync("Hello, " + e.Author.Username);
                    //await e.Message.RespondAsync(await requestResult.AffixRequest());

                    if (message.Contains("hi")) await e.Message.RespondAsync("Hello, " + e.Author.Username);
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}