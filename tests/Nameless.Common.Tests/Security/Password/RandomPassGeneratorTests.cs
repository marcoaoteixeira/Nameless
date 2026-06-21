using Nameless.Security.Password;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Security.Password;

[UnitTest]
public class RandomPassGeneratorTests {
    private static RandomPassGenerator CreateSut() => new();

    [Fact]
    public async Task GenerateAsync_WithDefaultArguments_ReturnsNonEmptyPassword() {
        // arrange
        var sut = CreateSut();
        var arguments = new Arguments();

        // act
        var result = await sut.GenerateAsync(arguments, CancellationToken.None);

        // assert
        Assert.False(string.IsNullOrEmpty(result));
    }

    [Theory]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    public async Task GenerateAsync_WithFixedLength_ReturnsPasswordOfExactLength(int length) {
        // arrange
        var sut = CreateSut();
        var arguments = new Arguments {
            MinLength = length,
            MaxLength = length
        };

        // act
        var result = await sut.GenerateAsync(arguments, CancellationToken.None);

        // assert
        Assert.Equal(length, result.Length);
    }

    [Fact]
    public async Task GenerateAsync_WithLengthRange_ReturnsPasswordWithinRange() {
        // arrange
        var sut = CreateSut();
        var arguments = new Arguments {
            MinLength = 6,
            MaxLength = 12
        };

        // act
        var result = await sut.GenerateAsync(arguments, CancellationToken.None);

        // assert
        Assert.InRange(result.Length, arguments.MinLength, arguments.MaxLength);
    }

    [Fact]
    public async Task GenerateAsync_WithUpperCaseCharsConfigured_ReturnsPasswordContainingAtLeastOneUpperCaseChar() {
        // arrange
        var sut = CreateSut();
        const string UpperCases = "ABCDEFGIJKMNOPQRSTWXYZ";

        // The generator cycles through all four character groups (lower, upper,
        // numeric, symbol) in blocks of four, so a password of length >= 4 will
        // always contain at least one character from each configured group.
        var arguments = new Arguments {
            MinLength = 8,
            MaxLength = 8,
            UpperCases = UpperCases
        };

        // act
        var result = await sut.GenerateAsync(arguments, CancellationToken.None);

        // assert
        Assert.Contains(result, UpperCases.Contains);
    }

    [Fact]
    public async Task GenerateAsync_WithNumericsConfigured_ReturnsPasswordContainingAtLeastOneDigit() {
        // arrange
        var sut = CreateSut();
        const string Numerics = "0123456789";

        var arguments = new Arguments {
            MinLength = 8,
            MaxLength = 8,
            Numerics = Numerics
        };

        // act
        var result = await sut.GenerateAsync(arguments, CancellationToken.None);

        // assert
        Assert.Contains(result, Numerics.Contains);
    }

    [Fact]
    public async Task GenerateAsync_WithSymbolsConfigured_ReturnsPasswordContainingAtLeastOneSymbol() {
        // arrange
        var sut = CreateSut();
        const string Symbols = "*$-+?_&=!%{}/";

        var arguments = new Arguments {
            MinLength = 8,
            MaxLength = 8,
            Symbols = Symbols
        };

        // act
        var result = await sut.GenerateAsync(arguments, CancellationToken.None);

        // assert
        Assert.Contains(result, Symbols.Contains);
    }

    [Fact]
    public async Task GenerateAsync_WithAllCharsFromAllowedSets_ReturnsPasswordContainingOnlyAllowedChars() {
        // arrange
        var sut = CreateSut();
        var arguments = new Arguments {
            MinLength = 12,
            MaxLength = 12
        };
        var allowedChars = arguments.LowerCases
            + arguments.UpperCases
            + arguments.Numerics
            + arguments.Symbols;

        // act
        var result = await sut.GenerateAsync(arguments, CancellationToken.None);

        // assert
        Assert.True(result.All(allowedChars.Contains));
    }

    [Theory]
    [InlineData(0, 8)]
    [InlineData(8, 0)]
    [InlineData(12, 6)]
    public async Task GenerateAsync_WithInvalidLengthRange_ThrowsArgumentOutOfRangeException(
        int minLength, int maxLength) {
        // arrange
        var sut = CreateSut();
        var arguments = new Arguments {
            MinLength = minLength,
            MaxLength = maxLength
        };

        // act
        var exception = await Record.ExceptionAsync(() => sut.GenerateAsync(arguments, CancellationToken.None));

        // assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    [Fact]
    public async Task GenerateAsync_WithCancelledToken_ReturnsEmptyString() {
        // arrange
        var sut = CreateSut();
        var arguments = new Arguments();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // act
        var result = await sut.GenerateAsync(arguments, cts.Token);

        // assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public async Task GenerateAsync_WithNullArguments_ThrowsArgumentNullException() {
        // arrange
        var sut = CreateSut();
        Arguments? nullArguments = null;

        // act
        var exception = await Record.ExceptionAsync(() => sut.GenerateAsync(nullArguments!, CancellationToken.None));

        // assert
        Assert.IsType<ArgumentNullException>(exception);
    }
}
