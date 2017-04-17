using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feedback.SentimentCalculator
{
    class TextAnalysisRequest
    {
        [JsonProperty("documents")]
        public IEnumerable<TextDocument> Documents { get; }

        public TextAnalysisRequest(IEnumerable<TextDocument> documents)
        {
            Documents = documents;
        }
    }

    class TextDocument
    {
        [JsonProperty("id")]
        public int Id { get; }
        [JsonProperty("text")]
        public string Text { get; }

        public TextDocument(int id, string text)
        {
            Id = id;
            Text = text;
        }
    }
}