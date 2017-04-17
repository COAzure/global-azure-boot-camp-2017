using Feedback.Data;
using Feedback.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Feedback.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Index()
        {
            var viewModel = new FeedbackListViewModel();
            using (var dbContext = new FeedbackDbContext())
            {
                try
                {
                    dbContext.Database.Initialize(false);
                    LoadFeedbackList(viewModel, dbContext);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Could not initialize database, probably because it doesn't exist yet. It'll get created by the VSTS Delivery Pipeline!\r\n{e}");
                }
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(FeedbackViewModel submitModel)
        {
            using (var dbContext = new FeedbackDbContext())
            {
                if (ModelState.IsValid)
                {
                    var model = new Data.Feedback
                    {
                        Content = submitModel.Content,
                        CreateTimestamp = DateTimeOffset.UtcNow,
                        EmailAddress = submitModel.EmailAddress
                    };
                    dbContext.Feedbacks.Add(model);
                    dbContext.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                var viewModel = new FeedbackListViewModel
                {
                    Content = submitModel.Content,
                    EmailAddress = submitModel.EmailAddress
                };
                LoadFeedbackList(viewModel, dbContext);
                return View(viewModel);
            }
        }

        private static void LoadFeedbackList(FeedbackListViewModel viewModel, FeedbackDbContext dbContext)
        {
            viewModel.SubmittedFeedbacks = dbContext.Feedbacks.Select(f => new FeedbackViewModel
            {
                Content = f.Content,
                EmailAddress = f.EmailAddress,
                SentimentScore = f.SentimentScore
            }).ToList();
        }
    }
}