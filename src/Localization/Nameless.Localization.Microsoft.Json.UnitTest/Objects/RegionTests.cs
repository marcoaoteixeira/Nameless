namespace Nameless.Localization.Microsoft.Json.Objects {
    public class RegionTests {
        [Test]
        public void TryGetValue_Should_Return_True_When_Region_Found() {
            // arrange
            var sut = new Region {
                Messages = [
                    new() { ID = "Message A", Text = "Message A" },
                    new() { ID = "Message B", Text = "Message B" },
                    new() { ID = "Message C", Text = "Message C" }
                ]
            };

            // act
            var found = sut.TryGetValue("Message C", out var actual);

            // assert
            Assert.Multiple(() => {
                Assert.That(found, Is.True);
                Assert.That(actual.ID, Is.EqualTo("Message C"));
            });
        }

        [Test]
        public void TryGetValue_Should_Return_False_When_Region_Not_Found() {
            // arrange
            var sut = new Region {
                Messages = [
                    new() { ID = "Message A", Text = "Message A" },
                    new() { ID = "Message B", Text = "Message B" },
                    new() { ID = "Message C", Text = "Message C" }
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

        [TestCase("Region A", "Region A", true)]
        [TestCase("Region A", "Region B", false)]
        public void Equals_Should_Return_Corresponding_Result(string nameX, string nameY, bool expected) {
            // arrange
            var regionX = new Region { Name = nameX };
            var regionY = new Region { Name = nameY };

            // act
            var actual = regionX.Equals(regionY);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("Region A", "Region A", true)]
        [TestCase("Region A", "Region B", false)]
        public void GetHashCode_Should_Return_Corresponding_Result(string nameX, string nameY, bool expected) {
            // arrange
            var regionX = new Region { Name = nameX };
            var regionY = new Region { Name = nameY };

            // act
            var hashCodeX = regionX.GetHashCode();
            var hashCodeY = regionY.GetHashCode();

            // assert
            Assert.That(hashCodeX == hashCodeY, Is.EqualTo(expected));
        }

        [Test]
        public void ToString_Should_Return_Name() {
            // arrange
            const string expected = "Region A";
            var sut = new Region { Name = expected };

            // act
            var actual = sut.ToString();

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
