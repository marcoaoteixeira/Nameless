using System.Globalization;
using Autofac;
using FluentAssertions;
using Nameless.FileStorage.System;
using Nameless.Localization.Json.UnitTest.Fixtures;

namespace Nameless.Localization.Json.UnitTest {
    public class LocalizationModuleTests {

        private static IContainer CreateContainer(Action<ContainerBuilder>? builder = default) {
            var inner = new ContainerBuilder();
            inner
                .RegisterInstance(NullApplicationContext.Instance);
            inner
                .RegisterModule<FileStorageModule>();
            inner
                .RegisterInstance(LocalizationOptions.Default);
            inner
                .RegisterModule<LocalizationModule>();
            inner
                .RegisterType<Service>();

            if (builder != default) {
                builder(inner);
            }

            return inner.Build();
        }

        [Test]
        public void LocalizationModule_Register_Resolver() {
            // act
            using var container = CreateContainer();
            var service = container.Resolve<Service>();

            // assert
            service.Should().NotBeNull();
        }

        [TestCase("pt-BR", "Olá Mundo!")]
        [TestCase("en-US", "Hello World!")]
        [TestCase("es-ES", "¡Hola Mundo!")]
        public void LocalizationModule_Resolve_Service_With_Localization_Dependency(string cultureName, string phrase) {
            // arrange
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureName);

            using var container = CreateContainer();
            var service = container.Resolve<Service>();

            // act
            var actual = service.Get("Hello World!");

            // assert
            actual.Should().Be(phrase);
        }
    }
}