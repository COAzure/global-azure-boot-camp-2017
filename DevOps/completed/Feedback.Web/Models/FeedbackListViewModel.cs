using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Feedback.Web.Models
{
    public class FeedbackListViewModel : FeedbackViewModel
    {
        public IEnumerable<FeedbackViewModel> SubmittedFeedbacks { get; set; }
        public double? AverageSubmittedSentiment => SubmittedFeedbacks?.Average(f => f.SentimentScore);
    }
}