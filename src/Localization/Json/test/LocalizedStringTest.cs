using Xunit;

namespace Nameless.Localization.Test {
    public class LocalizedStringTest {
        [Theory]
        [InlineData ("Test", null, null)]
        [InlineData ("Test", "Teste", null)]
        [InlineData ("Test {0}", "Teste {0}", new object[] { 12345 })]
        [InlineData ("Test {0}", null, null)]
        public void ImplicitCast_CastToString_GetTranslation (string text, string translation, object[] args) {
            // arrange
            LocalizedString localizedString;

            // act
            localizedString = new LocalizedString (text, translation, args);
            string cast = localizedString;

            // assert
            Assert.Equal (localizedString.ToString (), cast);
        }
    }
}