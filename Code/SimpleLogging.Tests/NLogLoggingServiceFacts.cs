using System;
using NLog;
using NLog.Targets;
using Shouldly;
using SimpleLogging.NLog;
using Xunit;

namespace SimpleLogging.Tests
{
    public class NLogLoggingServiceFacts
    {
        public class ConstructorFacts
        {
            [Fact]
            public void GivenANameOnly_Constructor_IsInstantiatedWithANullLogger()
            {
                // Arrange.
                const string name = "blah";

                // Act.
                var service = new NLogLoggingService(name);

                // Assert.
                service.ShouldNotBe(null);
                service.Name.ShouldBe(name);
                LogManager.Configuration.ShouldBe(null);
            }

            [Fact]
            public void GivenANameAndAnAddress_Constructor_IsInstantiatedWithATwoTarget()
            {
                // Arrange.
                const string name = "blah";
                const string address = "udp://1.2.3.4:9999";

                // Act.
                var service = new NLogLoggingService(name, address);

                // Assert.
                service.ShouldNotBe(null);
                service.Name.ShouldBe(name);
                LogManager.Configuration.ShouldNotBe(null);
                LogManager.Configuration.AllTargets.ShouldNotBeEmpty();
                LogManager.Configuration.AllTargets.Count.ShouldBe(2);
                LogManager.Configuration.LoggingRules.ShouldNotBeEmpty();
                LogManager.Configuration.LoggingRules.Count.ShouldBe(1);
            }

            [Fact]
            public void GivenANameAndATarget_Constructor_IsInstantiatedWithATwoTarget()
            {
                // Arrange.
                const string name = "blah";
                var target = new NLogViewerTarget
                {
                    Address = "udp://1.2.3.4:9999",
                    IncludeNLogData = false
                };

                // Act.
                var service = new NLogLoggingService(name, target);

                // Assert.
                service.ShouldNotBe(null);
                service.Name.ShouldBe(name);
                LogManager.Configuration.ShouldNotBe(null);
                LogManager.Configuration.AllTargets.ShouldNotBeEmpty();
                LogManager.Configuration.AllTargets.Count.ShouldBe(2);
                LogManager.Configuration.LoggingRules.ShouldNotBeEmpty();
                LogManager.Configuration.LoggingRules.Count.ShouldBe(1);
            }

            [Fact]
            public void GivenANullName_Constructor_ThrowsAnException()
            {
                // Arrange.

                // Act.
                var exception = Should.Throw<ArgumentNullException>(() => new NLogLoggingService(null));

                // Assert.
                exception.ShouldNotBe(null);
                exception.Message.ShouldBe("A logger 'name' is required so it's possible to filter the log results.\r\nParameter name: name");
            }

            [Fact]
            public void GivenANameButANullAddress_Constructor_ThrowsAnException()
            {
                // Arrange.

                // Act.
                var exception = Should.Throw<ArgumentNullException>(() => new NLogLoggingService("blah", (string) null));

                // Assert.
                exception.ShouldNotBe(null);
                exception.Message.ShouldBe("Value cannot be null.\r\nParameter name: address");
            }
        }

        public class ConfigureNLogViewTargetFacts
        {
            [Fact]
            public void GivenAnAddress_ConfigureNLogViewTarget_ReconfiguresTheLoggerSucessfully()
            {
                // Arrange.
                var service = new NLogLoggingService("blah");
                LogManager.Configuration.ShouldBe(null);

                // Act.
                service.ConfigureNLogViewerTarget("udp://1.2.3.4:9999");

                // Assert.
                LogManager.Configuration.ShouldNotBe(null);
                LogManager.Configuration.AllTargets.ShouldNotBeEmpty();
                LogManager.Configuration.AllTargets.Count.ShouldBe(2);
                LogManager.Configuration.LoggingRules.ShouldNotBeEmpty();
                LogManager.Configuration.LoggingRules.Count.ShouldBe(1);
            }
        }
    }
}