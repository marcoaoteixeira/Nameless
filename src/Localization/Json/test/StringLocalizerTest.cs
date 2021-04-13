using System;
using Nameless.Localization.Json;
using Nameless.Localization.Json.Schemas;
using Xunit;
using Xunit.Abstractions;

namespace Nameless.Localization.Test {
    public class StringLocalizerTest {
        private readonly ITestOutputHelper _output;
        public StringLocalizerTest (ITestOutputHelper output) {
            _output = output;
        }

        [Fact]
        public void Get_WithValidTextMessage_ReturnsTranslation () {
            // arrange
            var messageCollection = new MessageCollection ("Test.Test", new [] {
                new Message ("Test", new [] { "Teste" })
            });
            IStringLocalizer stringLocalizer = new StringLocalizer ("Test", "Test", "Test", messageCollection, DefaultPluralizationRuleProvider.DefaultRule);

            // act
            var translation = stringLocalizer.Get ("Test");

            // assert
            Assert.Equal ("Teste", translation);
        }

        [Theory]
        [InlineData (-1, "Nothing")]
        [InlineData (0, "Nothing")]
        [InlineData (1, "Something")]
        [InlineData (100, "Something")]
        public void Get_WithPluralForm_ReturnsPluralTranslation (int count, string expected) {
            // arrange
            var translations = new [] { "Nothing", "Something" };
            var pluralizationRule = DefaultPluralizationRuleProvider.DefaultRule;
            var messageCollection = new MessageCollection ("Test.Test", new [] {
                new Message ("Test", translations)
            });
            IStringLocalizer stringLocalizer = new StringLocalizer ("Test", "Test", "Test", messageCollection, pluralizationRule);

            // act
            var translation = stringLocalizer.Get ("Test", count);

            // assert

            _output.WriteLine ($"Translation {expected} for count {count}");
            Assert.Equal (expected, translation);
        }

        [Fact]
        public void Get_WithMessageAndArgs_ReturnsTranslation () {
            // arrange
            var expected = "This is a test for 123";
            var messageCollection = new MessageCollection ("Test.Test", new [] {
                new Message ("Test", new [] { "This is a test for {0}" })
            });
            IStringLocalizer stringLocalizer = new StringLocalizer ("Test", "Test", "Test", messageCollection, DefaultPluralizationRuleProvider.DefaultRule);

            // act
            var actual = stringLocalizer.Get ("Test", args : new object[] { 123 });

            // assert
            Assert.Equal (expected, actual);
        }
    }
}