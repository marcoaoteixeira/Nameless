namespace Nameless.Security;

public class PasswordParametersTests {
    [Fact]
    public void WhenConstructing_WithSpecificParameter_ThenCanRetrieveCorrectParameters() {
        // arrange
        const int MinLength = 5;
        const int MaxLength = 10;
        const string Symbols = "£$%";
        const string Numerics = "456";
        const string LowerCases = "jkl";
        const string UpperCases = "RTY";

        // act
        var sut = new PasswordParameters {
            MinLength = MinLength,
            MaxLength = MaxLength,
            Symbols = Symbols,
            Numerics = Numerics,
            LowerCases = LowerCases,
            UpperCases = UpperCases
        };

        // assert
        Assert.Multiple(() => {
            Assert.Equal(MinLength, sut.MinLength);
            Assert.Equal(MaxLength, sut.MaxLength);
            Assert.Equal(Symbols, sut.Symbols);
            Assert.Equal(Numerics, sut.Numerics);
            Assert.Equal(LowerCases, sut.LowerCases);
            Assert.Equal(UpperCases, sut.UpperCases);
        });
    }
}
