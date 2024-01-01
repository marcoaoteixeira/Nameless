namespace Nameless.Localization.Microsoft.Json.Objects {
    public class TranslationTests {
        [Test]
        public void TryGetValue_Should_Return_True_When_Region_Found() {
            // arrange
            var sut = new Translation {
                Regions = [
                    new Region { Name = "Test A" },
                    new Region { Name = "Test B" },
                    new Region { Name = "Test C" }
                ]
            };

            // act
            var found = sut.TryGetValue("Test B", out var actual);

            // assert
            Assert.Multiple(() => {
                Assert.That(found, Is.True);
                Assert.That(actual.Name, Is.EqualTo("Test B"));
            });
        }

        [Test]
        public void TryGetValue_Should_Return_False_When_Region_Not_Found() {
            // arrange
            var sut = new Translation {
                Regions = [
                    new Region { Name = "Test A" },
                    new Region { Name = "Test B" },
                    new Region { Name = "Test C" }
                ]
            };

            // act
            var found = sut.TryGetValue("Error", out var actual);

            // assert
            Assert.Multiple(() => {
                Assert.That(found, Is.False);
                Assert.That(actual, Is.Null);
            });
        }

        [TestCase("en-US", "en-US", true)]
        [TestCase("pt-BR", "en-US", false)]
        public void Equals_Should_Return_Corresponding_Result(string cultureX, string cultureY, bool expected) {
            // arrange
            var translationX = new Translation { Culture = cultureX };
            var translationY = new Translation { Culture = cultureY };

            // act
            var actual = translationX.Equals(translationY);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("en-US", "en-US", true)]
        [TestCase("pt-BR", "en-US", false)]
        public void GetHashCode_Should_Return_Corresponding_Result(string cultureX, string cultureY, bool expected) {
            // arrange
            var translationX = new Translation { Culture = cultureX };
            var translationY = new Translation { Culture = cultureY };

            // act
            var hashCodeX = translationX.GetHashCode();
            var hashCodeY = translationY.GetHashCode();

            // assert
            Assert.That(hashCodeX == hashCodeY, Is.EqualTo(expected));
        }

        [Test]
        public void ToString_Should_Return_Culture() {
            // arrange
            const string expected = "en-US";
            var translation = new Translation { Culture = expected };

            // act
            var actual = translation.ToString();

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
