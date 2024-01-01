namespace Nameless.Localization.Microsoft.Json.Objects {
    public class MessageTests {
        [TestCase("Message A", "Message A", true)]
        [TestCase("Message A", "Message B", false)]
        public void Equals_Should_Return_Corresponding_Result(string idX, string idY, bool expected) {
            // arrange
            var messageX = new Message { ID = idX };
            var messageY = new Message { ID = idY };

            // act
            var actual = messageX.Equals(messageY);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("Message A", "Message A", true)]
        [TestCase("Message A", "Message B", false)]
        public void GetHashCode_Should_Return_Corresponding_Result(string idX, string idY, bool expected) {
            // arrange
            var messageX = new Message { ID = idX };
            var messageY = new Message { ID = idY };

            // act
            var hashCodeX = messageX.GetHashCode();
            var hashCodeY = messageY.GetHashCode();

            // assert
            Assert.That(hashCodeX == hashCodeY, Is.EqualTo(expected));
        }

        [Test]
        public void ToString_Should_Return_ID_And_Text() {
            // arrange
            const string expected = "Message A : Message A";
            var sut = new Message {
                ID = "Message A",
                Text = "Message A"
            };

            // act
            var actual = sut.ToString();

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
