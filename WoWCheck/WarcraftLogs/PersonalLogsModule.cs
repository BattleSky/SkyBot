using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WoWCheck.Converters;

namespace WoWCheck.WarcraftLogs
{
    internal class PersonalLogsModule
    {
        public async Task<DiscordEmbedBuilder> PersonalLogsRequest(string name, string metric, string[] serverNameInput)
        {
            DiscordEmbedBuilder embed;

            var errorEmbed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#3AE6DB"),
                Title = "Ошибка запроса",
                Timestamp = DateTime.UtcNow,
            };
            try
            {
                var serverName = ServerName.WclServerNameConvert(serverNameInput);

                if (metric != "dps" && metric != "hps")
                    throw new ArgumentException("Указана неверная метрика. Требуется указать либо `dps` либо `hps` \n " +
                                                "Например: `-logs dps адэльвиль гордунни`");
                var encodedName = HttpUtility.UrlPathEncode(name);
                var responseContent = WclRequest.Request(
                    "https://www.warcraftlogs.com:443/v1/rankings/character/"
                    + encodedName + "/" + serverName
                    + "/eu?metric=" + metric + "&timeframe=historical").Result;

                using var reader = new StreamReader(await responseContent.ReadAsStreamAsync());
                List<PersonalLogsStats> serializedStats;
                try
                {
                    serializedStats = PersonalLogsStats.FromJson(await reader.ReadToEndAsync());
                }
                catch
                {
                    throw new Exception("Ошибка десериализации результатов от WarcraftLogs \nПроверьте, верно ли вы ввели никнейм и сервер?\n Синтаксис: `-logs метрика имя сервер`" +
                                        "\n Например, `-logs dps адэльвиль гордунни`");
                }
                embed = CreateEmbed(serializedStats, metric);
            }
            catch (Exception e)
            {
                errorEmbed.AddField("Ошибка:", e.Message);
                Console.WriteLine(e);
                return errorEmbed;
            }
            return embed;
        }
        public DiscordEmbedBuilder CreateEmbed(List<PersonalLogsStats> stats, string metric)
        {
            var metricLink = "healing";
            var metricName = "исцеления";
            if (metric == "dps")
            {
                metricLink = "damage-done";
                metricName = "нанесения урона";
            }
            
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#3AE6DB"),
                Title = "Статистика " + metricName + " WarcraftLogs",
                Timestamp = DateTime.UtcNow,
            };


            embed.WithFooter("(c) WarcraftLogs", "https://assets.rpglogs.com/img/warcraft/favicon.png");
            if (stats.Count == 0)
                throw new Exception("Не получилось загрузить информацию. \n" +
                                    "Возможно, за указанным персонажем отсутствуют закрепленные логи " +
                                    "или все его логи скрыты настройками приватности.");
            
            embed.AddField("Имя", stats[0].CharacterName + "\n" + stats[0].Server, true);
            embed.AddField("Класс", stats[0].Class + "\n", true);
            BestRunsToFields(FindMostDifficulty(stats), embed, metricLink);
            return embed;
        }

        public void BestRunsToFields(Dictionary<int, PersonalLogsStats> kills, 
            DiscordEmbedBuilder embed, string linkmetric)
        {
            foreach (var (_, singleKill) in kills)
            {
                var result = ("- **" + (int)singleKill.Percentile + "** - "
                              + singleKill.Spec
                              + ", Ур. предметов: **" + singleKill.IlvlKeyOrPatch
                              + "**, Позиция: *" + singleKill.Rank + "/" + singleKill.OutOf
                              + "*, Дата: "
                              + DateTimeOffset.FromUnixTimeMilliseconds(singleKill.StartTime).ToString("dd/MM/yy")
                              + ",  [Ссылка на бой](https://www.warcraftlogs.com/reports/" + singleKill.ReportId + "#fight="
                              + singleKill.FightId + "&type=" + linkmetric + ")");
                embed.AddField(singleKill.EncounterName + " *(" + (Difficulty)singleKill.Difficulty + ")*", result);
            }
        }
        private Dictionary<int, PersonalLogsStats> FindMostDifficulty(List<PersonalLogsStats> stats)
        {
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
            
            return TranslatorAndSerializer(resultDictionary);
        }
        private static Dictionary<int, PersonalLogsStats> TranslatorAndSerializer(
            Dictionary<long, PersonalLogsStats> sortedStats)
        {
            var rightDict = new Dictionary<int, PersonalLogsStats>();
            foreach (var (_,value) in sortedStats)
            {
                var (order, name) = BossNameWithOrder.BossNameSqlConverter(value.EncounterName);
                value.EncounterName = name;
                rightDict.Add(order, value);
            }
            return rightDict;
        }
    }
    
    public enum Difficulty
    {
        ПоискГруппы = 2,
        Нормальный,
        Героический,
        Эпохальный,
    }

    #region Автогенерированный код (объект парсинга json)

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