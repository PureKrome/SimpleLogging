using System;
using Nancy;
using Shouldly;
using SimpleLogging.Core;

namespace SimpleLogging.Samples.NancyFX.Modules
{
    public class HomeModule : NancyModule
    {
        private readonly ILoggingService _loggingService;

        public HomeModule(ILoggingService loggingService)
        {
            loggingService.ShouldNotBe(null);
            _loggingService = loggingService;

            Get["/"] = _ => GetHome();
        }

        private dynamic GetHome()
        {
            _loggingService.Trace("GetHome");

            _loggingService.Debug("Current DateTime: '{0}'", DateTime.UtcNow);

            return View["home"];
        }
    }
}