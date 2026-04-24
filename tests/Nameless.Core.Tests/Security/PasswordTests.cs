using Moq;
using Nameless.Security.Password;

namespace Nameless.Security;

public class PasswordTests {
    // --- Arguments defaults ---

    [Fact]
    public void Arguments_DefaultMinLength_IsSix() {
        // act
        var args = new Arguments();

        // assert
        Assert.Equal(6, args.MinLength);
    }

    [Fact]
    public void Arguments_DefaultMaxLength_IsTwelve() {
        // act
        var args = new Arguments();

        // assert
        Assert.Equal(12, args.MaxLength);
    }

    [Fact]
    public void Arguments_DefaultSymbols_IsNotEmpty() {
        // act
        var args = new Arguments();

        // assert
        Assert.NotEmpty(args.Symbols);
    }

    [Fact]
    public void Arguments_DefaultNumerics_ContainsDigits() {
        // act
        var args = new Arguments();

        // assert
        Assert.Contains("0", args.Numerics);
        Assert.Contains("9", args.Numerics);
    }

    // --- GeneratorExtensions.GenerateAsync ---

    [Fact]
    public async Task GenerateAsync_WithoutArguments_CallsGenerateWithDefaultArguments() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var mock = new Mock<IPassGenerator>();
        mock.Setup(g => g.GenerateAsync(It.IsAny<Arguments>(), ct))
            .ReturnsAsync("password123");

        // act
        var result = await mock.Object.GenerateAsync(ct);

        // assert
        Assert.Equal("password123", result);
        mock.Verify(g => g.GenerateAsync(It.IsAny<Arguments>(), ct), Times.Once);
    }
}
