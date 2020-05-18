using Moq;
using Nameless.FileStorage;
using Nameless.Localization.Json;
using Xunit;

namespace Nameless.Localization.Test {
    public class StringLocalizerFactoryTest {
        [Fact]
        public void Create_WithType_ReturnsStringLocalizer () {
            // arrange
            var fileProvider = new Mock<IFileStorage> ();
            var messageCollectionAggregationProvider = new MessageCollectionPackageProvider (fileProvider.Object);
            IStringLocalizerFactory factory = new StringLocalizerFactory (messageCollectionAggregationProvider, new PluralizationRuleProvider ());

            // act
            var stringLocalizer = factory.Create (typeof (StringLocalizerFactoryTest), "pt-BR");

            // assert
            Assert.NotNull (stringLocalizer);
        }
    }
}