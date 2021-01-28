using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WoWCheck.Converters;
using WoWCheck.RaiderIO;

namespace WoWCheck.WarcraftLogs
{
    class PersonalLogsModule
    {

        public async Task<DiscordEmbedBuilder> PersonalLogsRequest(string name, string metric, string[] serverNameInput)
        {
            string serverName;
            try
            {
                serverName = ServerName.WclServerNameConvert(serverNameInput);

                if (metric != "dps" && metric != "hps")
                    throw new ArgumentException("Указана неверная метрика. Требуется указать либо \"dps\" либо \"hps\" \n " +
                                                "Например: -logs dps адэльвиль гордунни");
            }
            catch (Exception e)
            {
                return new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#3AE6DB"),
                    Title = "Ошибка запроса",
                    Description = e.Message,
                    Timestamp = DateTime.UtcNow,
                };
            }

            var encodedName = HttpUtility.UrlPathEncode(name);
            
            var responseContent =
                WCLRequest.Request(
                        "https://www.warcraftlogs.com:443/v1/rankings/character/"
                        + encodedName + "/" + serverName
                        + "/eu?metric=" + metric + "&timeframe=historical").Result;

            using var reader = new StreamReader(await responseContent.ReadAsStreamAsync());
            var serializedStats = PersonalLogsStats.FromJson(await reader.ReadToEndAsync());
            // туть
            DiscordEmbedBuilder embed;
            try
            {
                embed = CreateEmbed(serializedStats, name, serverName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return embed;
        }

        public DiscordEmbedBuilder CreateEmbed(List<PersonalLogsStats> stats, string playername, string servername)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#3AE6DB"),
                Title = "Статистика логов WarcraftLogs",
                //Description = "11",
                Timestamp = DateTime.UtcNow,
            };

            embed.WithFooter("(c) WarcraftLogs", "https://assets.rpglogs.com/img/warcraft/favicon.png");
            if (stats[0].Error != null)
            {
                embed.AddField("Не получилось загрузить информацию", "Ответ сервера:\n" + stats[0].Error +
                                                                     "\n Проверьте верно ли вы ввели никнейм и сервер?\n Синтаксис:\n< -logs метрика имя сервер >" +
                                                                     "\n Например -logs dps адэльвиль гордунни");
                return embed;
            }
            embed.AddField("Имя/Сервер", playername + " " + servername, true);
            
            embed.AddField("Лучшие результаты на боссах последнего рейда", BestRuns(FindMostDifficulty(stats)));

            return embed;
        }
        public string BestRuns(List<PersonalLogsStats> killsAtMostDifficulty)
        {
            var result = new StringBuilder();
            foreach (var kill in killsAtMostDifficulty)
            {
                result.Append(kill.EncounterName + " " + (int)kill.Percentile + "\n");
            }

            return result.ToString();
        }

        // Не очень элегантная сортировка, думай ещё
        public List<PersonalLogsStats> FindMostDifficulty(List<PersonalLogsStats> stats)
        {
            var resultList = new List<PersonalLogsStats>();
            foreach (var e in stats)
            {
                foreach (var oneResult in resultList)
                {
                    if (oneResult.EncounterId == e.EncounterId)
                        if (oneResult.Difficulty < e.Difficulty)
                            resultList.Remove(oneResult); // не удаляет. Нужно чтобы было для разных спеков и удаление низких сложностей
                }
                resultList.Add(e);
            }

            return resultList;
        }

    }
    public enum Difficulty
    {
        LFR = 2,
        N,
        H,
        M,
    }

    public class SerializedStats
    {
        public string EncounterName { get; set; }
        public string 
    }

    #region Автогенерированный код

    public partial class PersonalLogsStats
    {

        [JsonProperty("encounterID")]
        public long EncounterId { get; set; }

        [JsonProperty("encounterName")]
        public string EncounterName { get; set; }

        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("spec")]
        public string Spec { get; set; }

        [JsonProperty("rank")]
        public long Rank { get; set; }

        [JsonProperty("outOf")]
        public long OutOf { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("startTime")]
        public long StartTime { get; set; }

        [JsonProperty("reportID")]
        public string ReportId { get; set; }

        [JsonProperty("fightID")]
        public long FightId { get; set; }

        [JsonProperty("difficulty")]
        public long Difficulty { get; set; }

        [JsonProperty("characterID")]
        public long CharacterId { get; set; }

        [JsonProperty("characterName")]
        public string CharacterName { get; set; }

        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("percentile")]
        public double Percentile { get; set; }

        [JsonProperty("ilvlKeyOrPatch")]
        public long IlvlKeyOrPatch { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonProperty("estimated")]
        public bool Estimated { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }

    public partial class PersonalLogsStats
    {
        public static List<PersonalLogsStats> FromJson(string json) => JsonConvert.DeserializeObject<List<PersonalLogsStats>>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<PersonalLogsStats> self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

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
