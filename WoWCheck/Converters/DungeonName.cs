using System;
using System.Collections.Generic;
using System.Text;

namespace WoWCheck.Converters
{
    class DungeonName
    {
        public static string DungeonNameSqlConverter(string name)
        {
            var sql = new DatabaseConnection();
            var dictionary = sql.Select("`translations`", "WHERE `default_name` = \"" + name + "\"", "ru");
            try
            {
                return dictionary["ru"][0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return name;
            }
        }

        // Конвертер можно и нужно превратить в базу данных
        [Obsolete]
        public static string DungeonNameConverter(string name)
        {
            var data = new Dictionary<string, string>
            {
                {"Spires of Ascension", "Шпили Перерождения"},
                {"Mists of Tirna Scithe", "Туманы Тирна Скитта"},
                {"Halls of Atonement", "Чертоги покаяния"},
                {"Theater of Pain", "Театр Боли"},
                {"The Necrotic Wake", "Смертельная тризна"},
                {"De Other Side", "Та Сторона"},
                {"Sanguine Depths", "Кровавые катакомбы"},
                {"Plaguefall", "Чумные каскады"}
            };
            try
            {
                return data[name];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return name;
            }
        }


    }

    
}
