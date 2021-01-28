using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace WoWCheck.RaiderIO
{

    class WCLRequest
    {
        private static readonly string wclKeyPath = @"WCLKey.tkey";
        public static async Task<HttpContent> Request(string url)
        {
            string wclToken;
            try
            {
                using StreamReader sr = new StreamReader(wclKeyPath);
                wclToken = await sr.ReadToEndAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Logs Read Key:" + e.Message);
                throw;
            }
            var urlWithToken = url + "&api_key=" + wclToken;
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(new HttpMethod("GET"), urlWithToken);
            request.Headers.TryAddWithoutValidation("accept", "application/json");
            var response = await httpClient.SendAsync(request);
            return response.Content;
        }
    }
}
