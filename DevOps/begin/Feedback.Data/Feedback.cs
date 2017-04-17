using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Feedback.Data
{
    public class Feedback
    {
        public int Id { get; set; }
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        [MaxLength(250)]
        public string Content { get; set; }
        [Display(Name = "Sentiment Score")]
        public double? SentimentScore { get; set; }
        public DateTimeOffset CreateTimestamp { get; set; }
    }
}