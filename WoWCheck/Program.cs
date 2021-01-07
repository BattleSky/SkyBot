using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;

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
            var connections = new Connections();
            var discord = connections.CreateClient();
            var requestResult = new APIRequest();


            discord.MessageCreated += async e =>
            {
                var message = e.Message.Content;
                if (e.Channel.Id != 241874656318062593) return;
                if (message.StartsWith("-")) // TODO: Есть возможность реализовать это по-другому, в доке описание
                {
                    if (message.Contains("affix")
                    ) // TODO: Есть возможность реализовать это по-другому, в доке описание
                    {
                        // TODO: Не засорять конструктором этот метод
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#FF0000"),
                            Title = "Аффиксы на этой неделе",
                            //Description = "something wrong",
                            Timestamp = DateTime.UtcNow
                        };
                        foreach (var (key, value) in requestResult.MakeRequest().Result) embed.AddField(key, value);
                        embed.WithFooter("by Raider.IO", discord.CurrentUser.AvatarUrl);

                        // TODO: Сюда только ответ

                        await e.Message.RespondAsync(embed: embed.Build());
                    }
                    //await e.Message.RespondAsync("Hello, " + e.Author.Username);
                    //await e.Message.RespondAsync(await requestResult.MakeRequest());

                    if (message.Contains("hi")) await e.Message.RespondAsync("Hello, " + e.Author.Username);
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}