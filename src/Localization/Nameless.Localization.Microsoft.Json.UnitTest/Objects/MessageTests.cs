namespace Nameless.Localization.Microsoft.Json.Objects {
    public class MessageTests {
        [TestCase("Message A", "Message A", true)]
        [TestCase("Message A", "Message B", false)]
        public void Equals_Should_Return_Corresponding_Result(string idX, string idY, bool expected) {
            // arrange
            var messageX = new Message(idX, string.Empty);
            var messageY = new Message(idY, string.Empty);

            // act
            var actual = messageX.Equals(messageY);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("Message A", "Message A", true)]
        [TestCase("Message A", "Message B", false)]
        public void GetHashCode_Should_Return_Corresponding_Result(string idX, string idY, bool expected) {
            // arrange
            var messageX = new Message(idX, string.Empty);
            var messageY = new Message(idY, string.Empty);

            // act
            var hashCodeX = messageX.GetHashCode();
            var hashCodeY = messageY.GetHashCode();

            // assert
            Assert.That(hashCodeX == hashCodeY, Is.EqualTo(expected));
        }
    }
}
