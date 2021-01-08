using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
// навязал скрипт
using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// TODO: Переписать для аффиксов по-нормальному

namespace WoWCheck
{
    class APIRequest
    {
        private string RawAffixes { get; set; }
        private Affixes Affixes { get; set; }

        // Запрос данных и возврат их в качестве поля 
        public async Task<Dictionary<string,string>> MakeRequest()
        {
            HttpContent responseContent;
            var result = new Dictionary<string,string>();
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
                RawAffixes = await reader.ReadToEndAsync();
                // Write the output to console
                Affixes = Affixes.FromJson(RawAffixes);

                foreach (var detail in Affixes.AffixDetails)
                {
                    result.Add(detail.Name, detail.Description);
                }
            }
            return result;
        }
    }

    // Атоматически сгенерированный код
    
    public partial class Affixes
    {
        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("leaderboard_url")]
        public Uri LeaderboardUrl { get; set; }

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
        public static Affixes FromJson(string json) => JsonConvert.DeserializeObject<Affixes>(json, WoWCheck.Converter.Settings);
    }

    //public static class Serialize
    //{
    //    public static string ToJson(this Affixes self) => JsonConvert.SerializeObject(self, WoWCheck.Converter.Settings);
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


}
