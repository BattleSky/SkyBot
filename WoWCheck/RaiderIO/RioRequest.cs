using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WoWCheck.RaiderIO
{
    class RioRequest
    {
        public static async Task<HttpContent> Request(string url)
        {
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(new HttpMethod("GET"), url);
            request.Headers.TryAddWithoutValidation("accept", "application/json");
            var response = await httpClient.SendAsync(request);
            return response.Content;
        }
    }
}
