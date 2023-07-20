using Autofac;
using Microsoft.Extensions.Options;
using Nameless.Logging.log4net.UnitTests.Fixtures;

namespace Nameless.Logging.log4net.UnitTests {

    public class LoggingModuleTests {

        [Test]
        public void LoggingModule_Can_Register_Logging_Infrastructure() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterModule<LoggingModule>();
            builder.RegisterInstance(Log4netOptions.Default);
            using var container = builder.Build();

            // act
            var loggerFactory = container.Resolve<ILoggerFactory>();

            // assert
            Assert.Multiple(() => {
                Assert.That(loggerFactory, Is.Not.Null);
            });
        }

        [Test]
        public void Inject_Logger_On_Service() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterModule<LoggingModule>();
            builder.RegisterInstance(Log4netOptions.Default);
            builder.RegisterType<ServiceWithLogger>().As<IService>();
            using var container = builder.Build();

            // act
            var service = container.Resolve<IService>();

            // assert
            Assert.That(service, Is.Not.Null);
        }
    }
}
