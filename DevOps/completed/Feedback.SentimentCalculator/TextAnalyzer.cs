using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Feedback.SentimentCalculator
{
    class TextAnalyzer
    {
        /// <summary>
        /// Azure portal URL.
        /// </summary>
        private const string BaseUrl = "https://westus.api.cognitive.microsoft.com/";

        private static readonly string AccountKey = ConfigurationManager.AppSettings["CognetiveServicesAccountKey"];

        public static async Task<TextAnalysisResponse> CalculateSentimentAsync(TextAnalysisRequest request)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);

                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", AccountKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Request body must be JSON format.
                var requestJson = JsonConvert.SerializeObject(request);

                // Detect sentiment via API call.
                var uri = "text/analytics/v2.0/sentiment";
                var response = await CallEndpointAsync(client, uri, requestJson);
                Console.WriteLine("\nDetect sentiment response:\n" + response);

                // Response body also comes back in JSON format.
                return JsonConvert.DeserializeObject<TextAnalysisResponse>(response);
            }
        }

        private static async Task<string> CallEndpointAsync(HttpClient client, string uri, string requestJson)
        {
            var requestData = Encoding.UTF8.GetBytes(requestJson);
            using (var content = new ByteArrayContent(requestData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(uri, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}