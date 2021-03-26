using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WoWCheck.WarcraftLogs
{
    internal static class WclRequest
    {
        private const string WclKeyPath = @"Connections/WCLKey.tkey";

        public static async Task<HttpContent> Request(string url)
        {
            string wclToken;
            try
            {
                using StreamReader sr = new StreamReader(WclKeyPath);
                wclToken = await sr.ReadToEndAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw new FileNotFoundException("Not found key file. Contact administrator.");
            }

            try
            {
                var urlWithToken = url + "&api_key=" + wclToken;
                using var httpClient = new HttpClient();
                using var request = new HttpRequestMessage(new HttpMethod("GET"), urlWithToken);
                request.Headers.TryAddWithoutValidation("accept", "application/json");
                var response = await httpClient.SendAsync(request);
                return response.Content;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Something wrong with the connection to WarcraftLogs API");
            }

        }
    }
}
