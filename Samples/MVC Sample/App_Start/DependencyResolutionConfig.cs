using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using SimpleLogging.Core;
using SimpleLogging.NLog;

namespace SimpleLogging.Samples.MVC
{
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
}