using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WoWCheck.Connections;

namespace WoWCheck.RaiderIO
{
    class AffixesModule
    {
        #region Запрос и обработка
        // Запрос данных и возврат их в качестве поля 
        public async Task<DiscordEmbedBuilder> AffixRequest()
        {
            
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#3AE6DB"),
                Title = "Модификаторы эпохальных подземелий",
                //Description = "11",
                Timestamp = DateTime.UtcNow
            };
            var plusAddiction= new[] {" (+2) ", " (+4) ", " (+7) ", " (+10) "};
            var responseContent =
                RioRequest.Request("https://raider.io/api/v1/mythic-plus/affixes?region=eu&locale=ru").Result;

            // Собираем ответ
            using var reader = new StreamReader(await responseContent.ReadAsStreamAsync());
            var serializedAffixes = Affixes.FromJson(await reader.ReadToEndAsync());
            long affix4 = 0;
            long affix7 = 0;

            for (var i = 0; i < serializedAffixes.AffixDetails.Length; i++)
            {
                var namePlusAffixLevel = plusAddiction[i] + serializedAffixes.AffixDetails[i].Name;
                embed.AddField(namePlusAffixLevel, serializedAffixes.AffixDetails[i].Description);
                switch (i)
                {
                    case 1: affix4 = serializedAffixes.AffixDetails[i].Id; break;
                    case 2: affix7 = serializedAffixes.AffixDetails[i].Id; break;
                }
            }
            embed.WithFooter("by Raider.IO", "https://cdnassets.raider.io/images/brand/Mark_2ColorWhite.png");
            embed.AddField("Аффиксы следующей недели:", NextWeekAffix(affix4, affix7));
            return embed;
        }

        private string NextWeekAffix(long affix4, long affix7)
        {
            if (affix7 == 0 || affix4  == 0)
                throw new ArgumentException("В NextWeekAffix пришел ID=0");

            var sql = new DatabaseConnection();
            var sqlResult = new Dictionary<string, string>();
            var resultColumns = new[] {"affix2", "affix4", "affix7", "affix10"};
            const string columnName = "week";
            try
            {
                var currentWeekAffixSelect = sql.Select("`affixes_schedule`", 
                    $"WHERE affix4 = {affix4} and affix7 = {affix7}", columnName);
                var weekAffixId = Convert.ToInt32(currentWeekAffixSelect[columnName][0]) + 1;

                //Open connection
                if (sql.OpenConnection() != true) return "";
                var selectNextWeekAffixIds = new MySqlCommand(
                    "SELECT AFX2.`name` as affix2, AFX4.`name` as affix4, AFX7.`name` as affix7, AFX10.`name` as affix10 " +
                    "FROM affixes_schedule SCH " +
                    "INNER JOIN affixes_names AFX2 on AFX2.affixes_id = SCH.affix2 " +
                    "INNER JOIN affixes_names AFX4 on AFX4.affixes_id = SCH.affix4 " +
                    "INNER JOIN affixes_names AFX7 on AFX7.affixes_id = SCH.affix7 " +
                    "INNER JOIN affixes_names AFX10 on AFX10.affixes_id = SCH.affix10 " +
                    $"WHERE week = IF({weekAffixId} > (SELECT count(*) FROM affixes_schedule), 1, {weekAffixId});",
                    sql.Connection);

                var dataReader = selectNextWeekAffixIds.ExecuteReader();

                while (dataReader.Read())
                {
                    foreach (var t in resultColumns)
                        sqlResult.Add(t, dataReader[t] + "");
                }
                dataReader.Close();
                sql.CloseConnection();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var result = sqlResult[resultColumns[0]] + ", "
                                                     + sqlResult[resultColumns[1]] + ", "
                                                     + sqlResult[resultColumns[2]] + ", "
                                                     + sqlResult[resultColumns[3]] + ".";

            return result;
        }


        #endregion

        #region Автогенерированный код (объект парсинга json)

        public partial class Affixes
        {
            [JsonProperty("affix_details")]
            public AffixDetail[] AffixDetails { get; set; }
        }

        public class AffixDetail
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

        }

        public partial class Affixes
        {
            public static Affixes FromJson(string json) => JsonConvert.DeserializeObject<Affixes>(json, Converter.Settings);
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
