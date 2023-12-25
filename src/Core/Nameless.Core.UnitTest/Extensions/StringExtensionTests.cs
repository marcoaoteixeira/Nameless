namespace Nameless {
    public class StringExtensionTests {

        // Diacritics, often loosely called 'accents', are the various little dots and
        // squiggles which, in many languages, are written above, below or on top of
        // certain letters of the alphabet to indicate something about their
        // pronunciation.
        [Test]
        public void RemoveDiacritics_Should_Remove_Diacritics_From_String() {
            // arrange
            var value = "â-ê-î-ô-û-ŵ-ŷ-ä-ë-ï-ö-ü-ẅ-ÿ-á-é-í-ó-ú-ẃ-ý-à-è-ì-ò-ù-ẁ-ỳ-ã-ç-õ-ñ";
            var expected = "a-e-i-o-u-w-y-a-e-i-o-u-w-y-a-e-i-o-u-w-y-a-e-i-o-u-w-y-a-c-o-n";
            // act

            var actual = StringExtension.RemoveDiacritics(value);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Repeat_Should_Repeat_String_X_Times() {
            // arrange
            var value = "Test";
            var times = 5;
            var expected = "TestTestTestTestTest";

            // act
            var actual = StringExtension.Repeat(value, times);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Repeat_Should_Not_Repeat_If_Times_Negative() {
            // arrange
            var value = "Test";
            var times = -5;
            var expected = "Test";

            // act
            var actual = StringExtension.Repeat(value, times);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
