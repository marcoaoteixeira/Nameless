using System.Collections.Generic;
using System.IO;
using System.Threading;
using Moq;
using Nameless.FileProvider;
using Nameless.Localization.Json;
using Xunit;

namespace Nameless.Localization.UnitTest {
    public class StringLocalizerFactoryTest {
        [Fact]
        public void Create_WithoutResourceFileForCulture_ReturnsEmptyStringLocalizer () {
            // arrange
            IStringLocalizerFactory factory;
            var settings = new LocalizationSettings ();
            var directory = new Mock<IDirectory> ();
            directory
                .Setup (_ => _.GetEnumerator ())
                .Returns (new List<IFile> ().GetEnumerator ());
            var fileProvider = new Mock<IFileProvider> ();
            fileProvider
                .Setup (_ => _.GetDirectory (It.IsAny<string> ()))
                .Returns (directory.Object);
            var pluralizationRuleProvider = new Mock<IPluralizationRuleProvider> ();

            // act
            factory = new StringLocalizerFactory (
                fileProvider.Object,
                pluralizationRuleProvider.Object,
                settings);

            var stringLocalizer = factory.Create (typeof (StringLocalizerFactoryTest));

            // assert
            Assert.NotNull (stringLocalizer);
            Assert.IsType<EmptyStringLocalizer> (stringLocalizer);
            Assert.Equal (typeof (StringLocalizerFactoryTest).Namespace, stringLocalizer.BaseName);
            Assert.Equal (typeof (StringLocalizerFactoryTest).Name, stringLocalizer.Location);
            Assert.Equal (Thread.CurrentThread.CurrentUICulture.Name, stringLocalizer.CultureName);
        }

        [Fact]
        public void Create_WithResourceFileForCulture_ReturnsStringLocalizer () {
            // arrange
            IStringLocalizerFactory factory;
            var settings = new LocalizationSettings ();
            var l10nFilePath = Path.Combine (typeof (StringLocalizerFactoryTest).Assembly.GetDirectoryPath (), settings.ResourceFolderPath, "pt-BR.json");
            var file = new Mock<IFile> ();
            file
                .Setup (_ => _.GetStream ())
                .Returns (new StreamReader (l10nFilePath).BaseStream);
            file
                .SetupGet (_ => _.Path)
                .Returns (l10nFilePath);
            var directory = new Mock<IDirectory> ();
            directory
                .Setup (_ => _.GetEnumerator ())
                .Returns (new List<IFile> (new [] { file.Object }).GetEnumerator ());
            var fileProvider = new Mock<IFileProvider> ();
            fileProvider
                .Setup (_ => _.GetDirectory (It.IsAny<string> ()))
                .Returns (directory.Object);
            var pluralizationRuleProvider = new Mock<IPluralizationRuleProvider> ();

            // act
            factory = new StringLocalizerFactory (
                fileProvider.Object,
                pluralizationRuleProvider.Object,
                settings);

            var stringLocalizer = factory.Create (typeof (StringLocalizerFactoryTest), "pt-BR");

            // assert
            Assert.NotNull (stringLocalizer);
            Assert.IsType<StringLocalizer> (stringLocalizer);
            Assert.Equal (typeof (StringLocalizerFactoryTest).Namespace, stringLocalizer.BaseName);
            Assert.Equal (typeof (StringLocalizerFactoryTest).Name, stringLocalizer.Location);
            Assert.Equal ("pt-BR", stringLocalizer.CultureName);
        }
    }
}