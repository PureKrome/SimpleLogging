using Nancy;
using Nancy.TinyIoc;
using SimpleLogging.Core;
using SimpleLogging.NLog;

namespace SimpleLogging.Samples.NancyFX
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            // NOTE: Use a specific constructor, which is why we have to use the delayed registration.
            var loggingService = new NLogLoggingService();
            container.Register<ILoggingService>((c, p) => loggingService);
        }
    }
}