using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feedback.SentimentCalculator
{
    class TextAnalysisResponse
    {
        [JsonProperty("documents")]
        public IEnumerable<SentimentDocument> Documents { get; set; }
    }

    class SentimentDocument
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
    }
}