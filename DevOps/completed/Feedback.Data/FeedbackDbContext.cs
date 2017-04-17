using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Feedback.Data
{
    public class FeedbackDbContext : DbContext
    {
        public DbSet<Feedback> Feedbacks { get; set; }
    }
}