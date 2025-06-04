namespace Nameless.Localization.Json.Objects;

public class MessageTests {
    [Theory]
    [InlineData("Message A", "Message A", true)]
    [InlineData("Message A", "Message B", false)]
    public void Equals_Should_Return_Corresponding_Result(string idX, string idY, bool expected) {
        // arrange
        var messageX = new Message(idX, string.Empty);
        var messageY = new Message(idY, string.Empty);

        // act
        var actual = messageX.Equals(messageY);

        // assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Message A", "Message A", true)]
    [InlineData("Message A", "Message B", false)]
    public void GetHashCode_Should_Return_Corresponding_Result(string idX, string idY, bool expected) {
        // arrange
        var messageX = new Message(idX, string.Empty);
        var messageY = new Message(idY, string.Empty);

        // act
        var hashCodeX = messageX.GetHashCode();
        var hashCodeY = messageY.GetHashCode();

        // assert
        Assert.Equal(expected, hashCodeX == hashCodeY);
    }
}