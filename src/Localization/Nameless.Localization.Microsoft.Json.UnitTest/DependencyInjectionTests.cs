using Autofac;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Moq;
using Nameless.Localization.Microsoft.Json.DependencyInjection;

namespace Nameless.Localization.Microsoft.Json {
    public class DependencyInjectionTests {
        [Test]
        public void Register_Resolve_Service() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterLocalizationModule();

            // We need an IFileProvider
            var fileProviderMock = new Mock<IFileProvider>();
            fileProviderMock
                .Setup(mock => mock.GetFileInfo(It.IsAny<string>()))
                .Returns(Mock.Of<IFileInfo>());
            builder
                .RegisterInstance(fileProviderMock.Object)
                .As<IFileProvider>();

            using var container = builder.Build();

            // act
            var factory = container.Resolve<IStringLocalizerFactory>();
            var localizer = container.Resolve<IStringLocalizer<Fake>>();

            // assert
            Assert.Multiple(() => {
                Assert.That(factory, Is.Not.Null);
                Assert.That(factory, Is.InstanceOf<StringLocalizerFactory>());

                Assert.That(localizer, Is.Not.Null);
                Assert.That(localizer, Is.InstanceOf<StringLocalizer<Fake>>());
            });
        }

        public class Fake { }
    }
}
