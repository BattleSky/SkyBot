using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Design;
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
                embed = CreateEmbed(serializedStats, metric);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return embed;
        }

        public DiscordEmbedBuilder CreateEmbed(List<PersonalLogsStats> stats, string metric)
        {
            var linkMetric = "healing-done";
            var metricName = "исцеления";
            if (metric == "dps")
            {
                linkMetric = "damage-done";
                metricName = "нанесения урона";
            }
            
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#3AE6DB"),
                Title = "Статистика " + metricName + " WarcraftLogs",
                //Description = "11",
                Timestamp = DateTime.UtcNow,
            };


            embed.WithFooter("(c) WarcraftLogs", "https://assets.rpglogs.com/img/warcraft/favicon.png");
            if (stats.Count == 0)
            {
                embed.AddField("Не получилось загрузить информацию",
                    "Возможно, за указанным персонажем отсутствуют закрепленные логи " +
                    "или все его логи скрыты настройками приватности.");
                embed.WithColor(new DiscordColor("#F90012"));
                return embed;
            }
            if (stats[0].Error != null)
            {
                embed.AddField("Не получилось загрузить информацию", "Ответ сервера:\n" + stats[0].Error +
                                                                     "\n Проверьте, верно ли вы ввели никнейм и сервер?\n Синтаксис:\n< -logs метрика имя сервер >" +
                                                                     "\n Например -logs dps адэльвиль гордунни");
                embed.WithColor(new DiscordColor("#F90012"));
                return embed;
            }
            embed.AddField("Имя", stats[0].CharacterName + "\n" + stats[0].Server, true);
            embed.AddField("Класс", stats[0].Class + "\n", true);
            BestRunsToFields(FindMostDifficulty(stats), embed, linkMetric);
            
            return embed;
        }
        //public string BestRuns(Dictionary<long, PersonalLogsStats> killsAtMostDifficulty)
        //{
        //    var result = new StringBuilder();
        //    foreach (var (_, value) in killsAtMostDifficulty)
        //    {
        //        Console.WriteLine(value.StartTime);
        //        result.Append("-- **" + (int) value.Percentile + "** " + value.EncounterName +
        //                      "(*" + (Difficulty) value.Difficulty + "*) **Ранг:** *"
        //                      + value.Rank + "/" + value.OutOf + "* " + value.Spec + " " + 
        //                      DateTimeOffset.FromUnixTimeMilliseconds(value.StartTime)
        //                    //  + "\n"
        //                      );
        //    }

        //    return result.ToString();
        //}

        public void BestRunsToFields(Dictionary<long, PersonalLogsStats> killsAtMostDifficulty, 
            DiscordEmbedBuilder embed, string linkmetric)
        {
            foreach (var (_, value) in killsAtMostDifficulty)
            {
                var result = ("- **" + (int) value.Percentile + "** - "
                              + value.Spec 
                              + ", Уровень предметов: **" + value.IlvlKeyOrPatch 
                              + "**, Ранг: *" + value.Rank + "/" + value.OutOf 
                              + "*, Дата: "
                              + DateTimeOffset.FromUnixTimeMilliseconds(value.StartTime).ToString("dd/MM/yy") 
                              + ",  [Ссылка на бой](https://www.warcraftlogs.com/reports/" + value.ReportId + "#fight=" 
                                + value.FightId + "&type=" + linkmetric + ")");
               embed.AddField(value.EncounterName + " *(" + (Difficulty)value.Difficulty + ")*", result);
            }
        }
        public Dictionary<long, PersonalLogsStats> FindMostDifficulty(List<PersonalLogsStats> stats)
        {
            // Для правильной сортировки нужно делать запросы в армори
            // возможно, выйдет с помощью подключения базы данных с fightid, encounterid и именем босса
            var resultDictionary = new Dictionary<long, PersonalLogsStats>();
            foreach (var e in stats)
            {
                if (resultDictionary.ContainsKey(e.EncounterId))
                {   
                    // Если совпадает encounterId и при этом записанная сложность ниже
                    // или процентиль ниже при той же сложности - удаляем из сортировки
                     if (resultDictionary[e.EncounterId].Difficulty < e.Difficulty ||
                            (resultDictionary[e.EncounterId].Percentile < e.Percentile && resultDictionary[e.EncounterId].Difficulty == e.Difficulty))
                        resultDictionary.Remove(e.EncounterId);
                     else continue;
                }
                resultDictionary.Add(e.EncounterId, e);
            }

            return resultDictionary;
        }

    }
    public enum Difficulty
    {
        ПоискГруппы = 2,
        Нормальный,
        Героический,
        Эпохальный,
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
//foreach (var oneResult in resultDictionary)
//{
//    if (resultDictionary.ContainsKey())
//        if (oneResult.Difficulty < e.Difficulty)
//            resultList.Remove(oneResult); // не удаляет. Нужно чтобы было для разных спеков и удаление низких сложностей
//}