![Simple Logging - making logging ... simple!](http://i.imgur.com/2IDUw6e.png)
----
**SimpleLogging** are a few .NET libraries that help making logging easier for your website/application.

It leverages the NLog framework for controlling how logging messages are sent.

### Getting Started

This is how easy it is to get started adding logging to your ASP.NET MVC application.

Step 1. Download & install [Sentinal](http://sentinel.codeplex.com/).

Step 2. Add the logging package.

![Nuget command line](http://i.imgur.com/NcM2Lie.png)

Step 3. Register the logging service interface with your Dependency Resolver.    
(This is using [AutoFac](http://autofac.org/) for IoC)    
```
public static class DependencyResolutionConfig
{
    public static void RegisterContainers()
    {
        var builder = new ContainerBuilder();

        // Register our services.
        builder.Register(c => new NLogLoggingService())
            .As<ILoggingService>();

        // Register our controllers (so they will use constructor injection)
        builder.RegisterControllers(typeof(MvcApplication).Assembly);

        var container = builder.Build();

        DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
    }
}
```
Step 4. Add some logging messages

```
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
```

Step 5. Add an `NLog.config` file.    
*Note:* Add this new file to the root website / application folder.

```
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      throwExceptions="true">

    <!-- NLog example: https://github.com/nlog/nlog/wiki/Examples -->
    <targets async="true">
        <target xsi:type="NLogViewer"
                name="sentinal" 
                address="udp://127.0.0.1:9999" />
    </targets>

    <rules>
        <logger name="*" minlevel="Trace" writeTo="sentinal"/>
    </rules>
    
</nlog>
```

Step 6. Run Sentinal

Step 7. Run the website.

![Zoh Mai Gawd](http://i.imgur.com/LNT9ys5.png)

----
I kindly accept Pull Requests :)
