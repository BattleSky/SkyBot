using System;
using WoWCheck.Connections;

namespace WoWCheck.Converters
{
    class BossNameWithOrder
    {
        public static Tuple<int, string> BossNameSqlConverter(string name)
        {
            var sql = new DatabaseConnection();
            var dictionary = sql.Select("`translations_bosses`", "WHERE `default_name` = \"" + name + "\"", "ru" , "order_in_dungeon");
            try
            {
                return Tuple.Create(Convert.ToInt32(dictionary["order_in_dungeon"][0]), dictionary["ru"][0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Tuple.Create(0, name);
            }
        }
    }
}
