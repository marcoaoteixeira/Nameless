namespace Nameless;

public class CharExtensionsTests {
    [Theory]
    [InlineData(true,
        new[] {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z'
        })]
    [InlineData(true,
        new[] {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u',
            'v', 'w', 'x', 'y', 'z'
        })]
    [InlineData(false,
        new[] {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '!', '@', '#', '$', '%', '¨', '&', '*', '(', ')', '-',
            '_', '=', '+'
        })]
    public void IsLetter_Returns_True_When_Letter(bool isOnlyLetter, char[] characteres) {
        // arrange
        var values = new bool[characteres.Length];
        // act
        for (var idx = 0; idx < values.Length; idx++) {
            values[idx] = characteres[idx].IsLetter();
        }

        var result = values.All(item => item == isOnlyLetter);

        // assert
        Assert.Equal(isOnlyLetter, result);
    }
}