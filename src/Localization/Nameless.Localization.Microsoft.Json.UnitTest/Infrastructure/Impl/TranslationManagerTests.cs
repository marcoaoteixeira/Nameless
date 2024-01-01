using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Nameless.Localization.Microsoft.Json.Infrastructure.Impl {
    public class TranslationManagerTests {
        private static Stream CreateFakeStream(string? culture = null) {
            var sb = new StringBuilder();

            sb.AppendLine("{");
            sb.AppendLine($"    \"Culture\": \"{culture}\",");
            sb.AppendLine("    \"Regions\": [{");
            sb.AppendLine($"			\"Name\": \"[{typeof(TranslationManagerTests).Assembly.GetName().Name}] {typeof(TranslationManagerTests).FullName}\",");
            sb.AppendLine("            \"Messages\": [{");
            sb.AppendLine("                    \"ID\": \"Message_ID\",");
            sb.AppendLine("					   \"Text\": \"Message_Value\"");
            sb.AppendLine("                }");
            sb.AppendLine("            ]");
            sb.AppendLine("        }");
            sb.AppendLine("    ]");
            sb.AppendLine("}");

            var value = sb.ToString();

            return new MemoryStream(Encoding.UTF8.GetBytes(value));
        }

        private static Mock<IFileInfo> CreateFileInfoMock(string? culture = null) {
            var fileInfoMock = new Mock<IFileInfo>();
            fileInfoMock
                .Setup(mock => mock.Exists)
                .Returns(true);
            fileInfoMock
                .Setup(mock => mock.CreateReadStream())
                .Returns(CreateFakeStream(culture ?? "pt-BR"));

            return fileInfoMock;
        }

        private static Mock<IDisposable> CreateFileChangeHandlerMock()
            => new();

        private static Mock<IChangeToken> CreateChangeTokenMock(IDisposable? fileChangeHandler = null) {
            var changeTokenMock = new Mock<IChangeToken>(); ;

            changeTokenMock
                .Setup(mock => mock.RegisterChangeCallback(
                    It.IsAny<Action<object?>>(),
                    It.IsAny<object?>()
                ))
                .Returns(fileChangeHandler ?? NullDisposable.Instance);

            return changeTokenMock;
        }

        private static Mock<IFileProvider> CreateFileProviderMock(IFileInfo? fileInfo = null, IChangeToken? changeToken = null) {
            var fileProviderMock = new Mock<IFileProvider>();

            if (fileInfo is not null) {
                fileProviderMock
                    .Setup(mock => mock.GetFileInfo(It.IsAny<string>()))
                    .Returns(fileInfo);
            }

            if (changeToken is not null) {
                fileProviderMock
                    .Setup(mock => mock.Watch(It.IsAny<string>()))
                    .Returns(changeToken);
            }

            return fileProviderMock;
        }

        [Test]
        public void GetTranslation_Should_Return_Translation_For_Given_Culture() {
            // arrange
            var fileProviderMock = CreateFileProviderMock(fileInfo: Mock.Of<IFileInfo>());
            var sut = new TranslationManager(fileProviderMock.Object);

            // act
            var translation = sut.GetTranslation("test");

            // assert
            Assert.That(translation, Is.Not.Null);
        }

        [Test]
        public void GetTranslation_Should_Return_Translation_For_Specified_Culture() {
            // arrange
            const string culture = "pt-BR";
            var options = new LocalizationOptions {
                WatchFileForChanges = false
            };
            var fileInfoMock = CreateFileInfoMock(culture);
            var fileProviderMock = CreateFileProviderMock(
                fileInfo: fileInfoMock.Object
            );
            var sut = new TranslationManager(fileProviderMock.Object, options);

            // act
            var translation = sut.GetTranslation(culture);

            // assert
            Assert.Multiple(() => {
                Assert.That(translation, Is.Not.Null);
                Assert.That(translation.Culture, Is.EqualTo(culture));
            });
        }

        [Test]
        public void GetTranslation_With_Watch_File_Option_Should_Return_Translation_For_Specified_Culture_And_Watch_File_For_Changes() {
            // arrange
            const string culture = "pt-BR";
            var options = new LocalizationOptions {
                WatchFileForChanges = true
            };
            var fileInfoMock = CreateFileInfoMock(culture);
            var changeTokenMock = CreateChangeTokenMock();
            var fileProviderMock = CreateFileProviderMock(
                fileInfo: fileInfoMock.Object,
                changeToken: changeTokenMock.Object
            );
            var sut = new TranslationManager(fileProviderMock.Object, options);

            // act
            var translation = sut.GetTranslation(culture);

            // assert
            Assert.Multiple(() => {
                Assert.That(translation, Is.Not.Null);
                Assert.That(translation.Culture, Is.EqualTo(culture));
                fileProviderMock.Verify(mock => mock.Watch(It.IsAny<string>()));
                changeTokenMock.Verify(mock => mock.RegisterChangeCallback(
                    It.IsAny<Action<object?>>(),
                    It.IsAny<object?>()
                ));
            });
        }

        [Test]
        public void Trigger_FileChangeHandler() {
            // arrange
            const string culture = "pt-BR";
            var options = new LocalizationOptions {
                WatchFileForChanges = true
            };
            var fileInfoMock = CreateFileInfoMock(culture);
            var fileChangeHandlerMock = CreateFileChangeHandlerMock();
            var changeToken = new FakeChangeToken(fileChangeHandlerMock.Object);
            var fileProviderMock = CreateFileProviderMock(
                fileInfo: fileInfoMock.Object,
                changeToken: changeToken
            );
            var sut = new TranslationManager(fileProviderMock.Object, options);

            // act
            var translation = sut.GetTranslation(culture);
            changeToken.Trigger();

            // assert
            Assert.Multiple(() => {
                Assert.That(translation, Is.Not.Null);
                Assert.That(translation.Culture, Is.EqualTo(culture));
                fileProviderMock.Verify(mock => mock.Watch(It.IsAny<string>()));
                fileChangeHandlerMock.Verify(mock => mock.Dispose());
            });
        }
    }
}
