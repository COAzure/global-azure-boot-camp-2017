using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Feedback.Data
{
    public class FeedbackDbContext : DbContext
    {
        private const string ConnectionStringName = "FeedbackDatabaseConnection";

        public DbSet<Feedback> Feedbacks { get; set; }

        private FeedbackDbContext() : base() { }

        private FeedbackDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        public static FeedbackDbContext Create()
        {
            if (ConfigurationManager.ConnectionStrings[ConnectionStringName] == null)
            {
                return new FeedbackDbContext();
            }
            return new FeedbackDbContext(ConnectionStringName);
        }
    }
}