using Feedback.Data;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feedback.SentimentCalculator
{
    class Program
    {
        private const string BaseUrl = "https://westus.api.cognitive.microsoft.com/";
        private const string Every1Minute = "0 */1 * * * *";

        static void Main(string[] args)
        {
            JobHostConfiguration config = CreateJobHostConfiguration();
            var host = new JobHost(config);
            host.RunAndBlock();
        }

        private static JobHostConfiguration CreateJobHostConfiguration()
        {
            var config = new JobHostConfiguration();
            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }
            config.UseTimers();
            return config;
        }

        public static async Task TriggerCalculateSentimentAsync([TimerTrigger(Every1Minute, RunOnStartup = false)] TimerInfo info)
        {
            using (var dbContext = new FeedbackDbContext())
            {
                await CalculateSentimentAsync(dbContext);
            }
        }

        private static async Task CalculateSentimentAsync(FeedbackDbContext dbContext)
        {
            // NOTE: Cognetive services accepts up to 1000 "documents" at a time.
            var feedbackBatch = dbContext.Feedbacks.Where(f => !f.SentimentScore.HasValue).Take(1000).ToList();
            if (feedbackBatch.Any())
            {
                Console.WriteLine("Found feedback to analyze...");
                var documents = feedbackBatch.Select(f => new TextDocument(f.Id, f.Content));
                var request = new TextAnalysisRequest(documents);
                var response = await TextAnalyzer.CalculateSentimentAsync(request);
                foreach (var sentiment in response.Documents)
                {
                    var feedback = feedbackBatch.Single(f => f.Id == sentiment.Id);
                    feedback.SentimentScore = sentiment.Score;
                }
                await dbContext.SaveChangesAsync();
                await CalculateSentimentAsync(dbContext); // Recursively process until we are done.
            }
            else
            {
                Console.WriteLine("Did not find feedback to analyze.");
            }
        }
    }
}