using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WoWCheck.Converters;

namespace WoWCheck.RaiderIO
{
    class MythicPlusModule
    {
        #region Запрос и обработка

        public async Task<DiscordEmbedBuilder> MythicPlusRequest(string name, string[] serverNameInput)
        {
            string serverName;
            try
            {
                serverName = ServerName.RioServerNameConvert(serverNameInput);
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

            var encodedName = HttpUtility.UrlPathEncode(name.First().ToString().ToUpper() + name.Substring(1));

            var responseContent =
                RioRequest.Request(
                        "https://raider.io/api/v1/characters/profile?region=eu&realm="
                        + serverName + "&name=" +encodedName
                        + "&fields=mythic_plus_scores_by_season%3Acurrent%2Cmythic_plus_best_runs%3Aall").Result;
            using var reader = new StreamReader(await responseContent.ReadAsStreamAsync());
            var serializedStats = MythicPlusStats.FromJson(await reader.ReadToEndAsync());
            DiscordEmbedBuilder embed;
            try
            {
                embed = CreateEmbed(serializedStats);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return embed;
        }

        public DiscordEmbedBuilder CreateEmbed(MythicPlusStats stats)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#3AE6DB"),
                Title = "Статистика Raider.IO",
                //Description = "11",
                Timestamp = DateTime.UtcNow,
            };

            embed.WithFooter("(c) Raider.IO", "https://cdnassets.raider.io/images/brand/Mark_2ColorWhite.png");
            if (stats.ErrorMessage != null)
            {
                embed.AddField("Не получилось загрузить информацию", "Ответ сервера:\n" + stats.ErrorMessage + 
                                                                     "\n Проверьте верно ли вы ввели никнейм и сервер?\n Синтаксис:\n< -rio имя сервер >");
                return embed;
            }
            embed.AddField("Имя", stats.Name, true);
            embed.AddField("Сервер", stats.Realm, true);
            embed.AddField("Специализация", stats.ActiveSpecName + " " + stats.Class, true);
            var scores = stats.MythicPlusScoresBySeason[0].Scores;
            embed.AddField("Рейтинг м+", "Урон: **" + scores.Dps + "**", true);
            embed.AddField("-", "Исцеление: **" + scores.Healer + "**", true);
            embed.AddField("-", "Танк: **" + scores.Tank + "**", true);
            embed.AddField("Лучшие пройденные", BestRuns(stats));

            return embed;
        }

        public string BestRuns(MythicPlusStats stats)
        {
            var collectInformation = new StringBuilder();
            foreach (var bestRun in stats.MythicPlusBestRuns)
            {
                var dungeonInRus = DungeonName.DungeonNameSqlConverter(bestRun.Dungeon);
                collectInformation.Append("**(" + bestRun.MythicLevel + "+" + bestRun.NumKeystoneUpgrades + ")** " + //bestRun.ShortName  +
                                          " *" + dungeonInRus + "*\n");
            }
            var result = collectInformation.ToString();
            if (result == "" || result == null)
                result = "Персонаж не закрывал значимых подземелий с ключом";
            return result;
        }


        #endregion

        #region Автогенерированный код

        public partial class MythicPlusStats
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("race")]
            public string Race { get; set; }

            [JsonProperty("class")]
            public string Class { get; set; }

            [JsonProperty("active_spec_name")]
            public string ActiveSpecName { get; set; }

            [JsonProperty("active_spec_role")]
            public string ActiveSpecRole { get; set; }

            [JsonProperty("gender")]
            public string Gender { get; set; }

            [JsonProperty("faction")]
            public string Faction { get; set; }

            [JsonProperty("achievement_points")]
            public long AchievementPoints { get; set; }

            [JsonProperty("honorable_kills")]
            public long HonorableKills { get; set; }

            [JsonProperty("thumbnail_url")]
            public Uri ThumbnailUrl { get; set; }

            [JsonProperty("region")]
            public string Region { get; set; }

            [JsonProperty("realm")]
            public string Realm { get; set; }

            [JsonProperty("last_crawled_at")]
            public DateTimeOffset LastCrawledAt { get; set; }

            [JsonProperty("profile_url")]
            public Uri ProfileUrl { get; set; }

            [JsonProperty("profile_banner")]
            public string ProfileBanner { get; set; }

            [JsonProperty("mythic_plus_scores_by_season")]
            public List<MythicPlusScoresBySeason> MythicPlusScoresBySeason { get; set; }

            [JsonProperty("mythic_plus_best_runs")]
            public List<MythicPlusBestRun> MythicPlusBestRuns { get; set; }

            [JsonProperty("message")]
            public string ErrorMessage { get; set; }
        }

        public partial class MythicPlusBestRun
        {
            [JsonProperty("dungeon")]
            public string Dungeon { get; set; }

            [JsonProperty("short_name")]
            public string ShortName { get; set; }

            [JsonProperty("mythic_level")]
            public long MythicLevel { get; set; }

            [JsonProperty("completed_at")]
            public DateTimeOffset CompletedAt { get; set; }

            [JsonProperty("clear_time_ms")]
            public long ClearTimeMs { get; set; }

            [JsonProperty("num_keystone_upgrades")]
            public long NumKeystoneUpgrades { get; set; }

            [JsonProperty("map_challenge_mode_id")]
            public long MapChallengeModeId { get; set; }

            [JsonProperty("score")]
            public double Score { get; set; }

            [JsonProperty("affixes")]
            public List<Affix> Affixes { get; set; }

            [JsonProperty("url")]
            public Uri Url { get; set; }
        }

        public partial class Affix
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

        public partial class MythicPlusScoresBySeason
        {
            [JsonProperty("season")]
            public string Season { get; set; }

            [JsonProperty("scores")]
            public Scores Scores { get; set; }
        }

        public partial class Scores
        {
            [JsonProperty("all")]
            public long All { get; set; }

            [JsonProperty("dps")]
            public long Dps { get; set; }

            [JsonProperty("healer")]
            public long Healer { get; set; }

            [JsonProperty("tank")]
            public long Tank { get; set; }

            [JsonProperty("spec_0")]
            public long Spec0 { get; set; }

            [JsonProperty("spec_1")]
            public long Spec1 { get; set; }

            [JsonProperty("spec_2")]
            public long Spec2 { get; set; }

            [JsonProperty("spec_3")]
            public long Spec3 { get; set; }
        }

        public partial class MythicPlusStats
        {
            public static MythicPlusStats FromJson(string json) => JsonConvert.DeserializeObject<MythicPlusStats>(json, Converter.Settings);
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
}
