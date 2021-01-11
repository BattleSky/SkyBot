using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WoWCheck.RaiderIO
{
    class AffixesModule
    {
        private Affixes SerializedAffixes { get; set; }

        #region Запрос и обработка
        // Запрос данных и возврат их в качестве поля 
        public async Task<Dictionary<string, string>> MakeRequest()
        {
            HttpContent responseContent;
            var result = new Dictionary<string, string>();
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://raider.io/api/v1/mythic-plus/affixes?region=eu&locale=ru"))
                {
                    request.Headers.TryAddWithoutValidation("accept", "application/json");
                    var response = await httpClient.SendAsync(request);
                    responseContent = response.Content;
                }
            }
            // Get the stream of the content.
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                SerializedAffixes = Affixes.FromJson(await reader.ReadToEndAsync());

                foreach (var detail in SerializedAffixes.AffixDetails)
                {
                    result.Add(detail.Name, detail.Description);
                }
            }
            return result;
        }

        #endregion

        #region Конструкторы блоков и ответов



        #endregion

        #region Автоматически сгенерированный класс


        public partial class Affixes
        {
            [JsonProperty("region")]
            public string Region { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            //[JsonProperty("leaderboard_url")]
            //public Uri LeaderboardUrl { get; set; }

            [JsonProperty("affix_details")]
            public AffixDetail[] AffixDetails { get; set; }
        }

        public partial class AffixDetail
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("wowhead_url")]
            public Uri WowheadUrl { get; set; }
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
