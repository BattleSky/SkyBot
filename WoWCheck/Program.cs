using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace WoWCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            MainTask(args).GetAwaiter().GetResult();
        }

        static async Task MainTask(string[] args)
        {
            var connections = new Connections();
            var discord = connections.CreateClient();
            var requestResult = new APIRequest();

            discord.MessageCreated += async e =>
            {
                var message = e.Message.Content;
                //if (e.Channel.Id != 4112314) return;
                if (message.StartsWith("!")) // TODO: Есть возможность реализовать это по-другому, в доке описание
                {
                    if (message.Contains("rio")) // TODO: Есть возможность реализовать это по-другому, в доке описание
                    {
                        // TODO: Не засорять конструктором этот класс
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#FF0000"),
                            Title = "Аффиксы на этой неделе",
                            //Description = await requestResult.MakeRequest(),
                            Timestamp = DateTime.UtcNow
                        };
                        foreach (var detail in requestResult.MakeRequest().Result)
                        {
                            embed.AddField(detail.Key, detail.Value);
                        }
                        embed.WithFooter(discord.CurrentUser.Username, discord.CurrentUser.AvatarUrl);

                        // TODO: Сюда только ответ
                        await e.Message.RespondAsync(embed: embed.Build());

                    }
                    //await e.Message.RespondAsync("Hello, " + e.Author.Username);
                    //await e.Message.RespondAsync(await requestResult.MakeRequest());
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
