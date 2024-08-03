using System.Text;

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

        [Test]
        public void ToStream_Should_Create_A_MemoryStream_From_A_String() {
            // arrange
            const string value = "This is a Test";

            // act
            using var actual = StringExtension.ToMemoryStream(value);

            // assert
            Assert.Multiple(() => {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.ToArray(), Is.Not.Empty);
                Assert.That(actual, Is.InstanceOf<MemoryStream>());
            });
        }

        [Test]
        public void CamelFriendly_Should_Create_A_Space_Separated_String_From_A_Camel_String_Value() {
            // arrange
            const string value = "ThisIsATest";
            const string expected = "This Is A Test";

            // act
            var actual = StringExtension.CamelFriendly(value);

            // arrange
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Ellipsize_Should_Return_A_Portion_Of_A_String_Ending_With_Ellipsis() {
            // arrange
            const string value = "This Is A Test";
            const string ellipsis = "...";
            const string expected = $"This Is{ellipsis}";
            const int count = 8;

            // act
            var actual = StringExtension.Ellipsize(value, count, ellipsis, wordBoundary: false);

            // arrange
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void FromHexToByteArray_Should_Return_Array_Of_Bytes() {
            // arrange
            const string expected = "Test Is A Test";
            var array = expected.ToCharArray()
                                .Select(item => ((int)item).ToString("X2"));
            var value = string.Join(string.Empty, array);

            // act
            var actual = StringExtension.FromHexToByteArray(value);

            // assert
            Assert.That(expected, Is.EqualTo(Encoding.UTF8.GetString(actual)));
        }

        [Test]
        public void ReplaceAll_Use_Dictionary_To_Replace_Values_Inside_Text() {
            // arrange
            var replacements = new Dictionary<string, string> {
                { "cat", "dog" },
                { "meow", "bark" }
            };
            const string phrase = "The cat goes meow";
            const string expected = "The dog goes bark";

            // act
            var actual = StringExtension.ReplaceAll(phrase, replacements);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ToBase64_Is_Just_A_Syntax_Sugar_For_Convert_ToBase64String() {
            // arrange
            const string expected = "VGhpcyBpcyBhIHRlc3Q="; // This is a test
            const string phrase = "This is a test";

            // act
            var actual = StringExtension.ToBase64(phrase);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ToBase64_Using_Different_Encoding() {
            // arrange
            const string expected = "VGhpcyBpcyBhIHRlc3Q="; // This is a test
            const string phrase = "This is a test";

            // act
            var actual = StringExtension.ToBase64(phrase, Encoding.ASCII);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
