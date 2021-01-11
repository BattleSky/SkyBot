using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WoWCheck.RaiderIO
{
    class AffixesModule
    {
        #region Запрос и обработка
        // Запрос данных и возврат их в качестве поля 
        public async Task<DiscordEmbedBuilder> AffixRequest(string avatarUrl)
        {
            HttpContent responseContent;
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#3AE6DB"),
                Title = "Модификаторы эпохальных подземелий",
                //Description = "11",
                Timestamp = DateTime.UtcNow
            };
            var plusAddiction= new[] {" (+2) ", " (+4) ", " (+7) ", " (+10) "};

            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://raider.io/api/v1/mythic-plus/affixes?region=eu&locale=ru"))
                {
                    request.Headers.TryAddWithoutValidation("accept", "application/json");
                    var response = await httpClient.SendAsync(request);
                    responseContent = response.Content;
                }
            }
            // Собираем ответ
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                var serializedAffixes = Affixes.FromJson(await reader.ReadToEndAsync());
                var counter = 0;
                foreach (var detail in serializedAffixes.AffixDetails)
                {   
                    var namePlusAffixLevel = plusAddiction[counter] + detail.Name;
                    embed.AddField(namePlusAffixLevel, detail.Description);
                    counter++;
                }
                embed.WithFooter("by Raider.IO", avatarUrl);
                
                return embed;
            }
        }

        #endregion

        #region Классы данных

        //Автоматически сгенерированные классы
        //Закоментирована неиспользуемая информация
        public partial class Affixes
        {
            //[JsonProperty("region")]
            //public string Region { get; set; }

            //[JsonProperty("title")]
            //public string Title { get; set; }

            //[JsonProperty("leaderboard_url")]
            //public Uri LeaderboardUrl { get; set; }

            [JsonProperty("affix_details")]
            public AffixDetail[] AffixDetails { get; set; }
        }

        public partial class AffixDetail
        {
            //[JsonProperty("id")]
            //public long Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            //[JsonProperty("wowhead_url")]
            //public Uri WowheadUrl { get; set; }
        }

        public partial class Affixes
        {
            public static Affixes FromJson(string json) => JsonConvert.DeserializeObject<Affixes>(json, Converter.Settings);
        }

        //public static class Serialize
        //{
        //    public static string ToJson(this SerializedAffixes self) => JsonConvert.SerializeObject(self, WoWCheck.Converter.Settings);
        //}

        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
                {
                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
                },
            };
        }

        #endregion

        }

    }
