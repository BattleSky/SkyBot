using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace WoWCheck.Converters
{
    class ServerName
    {
        public static string RioServerNameConvert(string[] servername)
        {
            if (servername.Length > 4)
                throw new ArgumentException("В имени сервера слишком много слов???");
            
            var result = new StringBuilder();
            var regexPattern = @"^[A-Za-zА-Яа-яЁё' -]{2,40}$";

            for (var i = 0; i < servername.Length; i++)
            {
                if (!Regex.IsMatch(servername[i], regexPattern))
                    throw new ArgumentException("Использованы неподдерживаемые символы или имя было слишком длинное");
                if (servername[i] == "король-лич" || servername[i] == "Король-лич")
                {
                    var justForLichKing = servername[i].First().ToString().ToUpper() + servername[i].Substring(1);
                    result.Append(justForLichKing);
                    continue;
                }

                result.Append(servername[i].ToLower());
                if (i + 1 != servername.Length)
                    result.Append("-");
            }


            return HttpUtility.UrlPathEncode(result.ToString());
        }


        public static string WclServerNameConvert(string[] servername)
        {
            if (servername.Length > 4)
                throw new ArgumentException("В имени сервера слишком много слов???");

            var result = new StringBuilder();
            var regexPattern = @"^[A-Za-zА-Яа-яЁё' -]{2,40}$";

            for (var i = 0; i < servername.Length; i++)
            {
                if (!Regex.IsMatch(servername[i], regexPattern))
                    throw new ArgumentException("Использованы неподдерживаемые символы или имя было слишком длинное");
                if (servername[i].Contains("-") || servername[i].Contains("'"))
                {
                    servername[i] = servername[i].Replace("-", "");
                    servername[i] = servername[i].Replace("'", "");
                }

                result.Append(servername[i].ToLower());
                if (i + 1 != servername.Length)
                    result.Append("-");
            }
            return HttpUtility.UrlPathEncode(result.ToString());
        }

    }
}
