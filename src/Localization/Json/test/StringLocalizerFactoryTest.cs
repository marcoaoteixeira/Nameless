using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;
using Moq;
using Nameless.Localization.Json;
using Xunit;

namespace Nameless.Localization.Test {
    public class StringLocalizerFactoryTest {
        [Fact]
        public void Create_WithType_ReturnsStringLocalizer () {
            // arrange
            var fileProvider = new PhysicalFileProvider (typeof (StringLocalizerFactoryTest).Assembly.GetDirectoryPath ());
            var messageCollectionAggregationProvider = new MessageCollectionPackageProvider (fileProvider);
            IStringLocalizerFactory factory = new StringLocalizerFactory (messageCollectionAggregationProvider, new PluralizationRuleProvider ());

            // act
            var stringLocalizer = factory.Create (typeof (StringLocalizerFactoryTest), "pt-BR");

            // assert
            Assert.NotNull (stringLocalizer);
        }
    }
}