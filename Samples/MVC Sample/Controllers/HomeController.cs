using System;
using System.Web.Mvc;
using Shouldly;
using SimpleLogging.Core;

namespace SimpleLogging.Samples.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILoggingService _loggingService;

        public HomeController(ILoggingService loggingService)
        {
            loggingService.ShouldNotBe(null);
            _loggingService = loggingService;
        }

        //
        // GET: /Home/

        public ActionResult Index()
        {
            _loggingService.Trace("GetHome");

            _loggingService.Debug("Current DateTime: '{0}'", DateTime.UtcNow);

            return View();
        }
    }
}