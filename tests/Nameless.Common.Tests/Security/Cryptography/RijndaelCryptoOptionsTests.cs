using Nameless.Testing.Tools.Attributes;

namespace Nameless.Security.Cryptography;

[UnitTest]
public class RijndaelCryptoOptionsTests {
    [Fact]
    public void KeyIterationsCount_DefaultValue_IsOne() {
        // arrange & act
        var sut = new RijndaelCryptoOptions();

        // assert
        Assert.Equal(1, sut.KeyIterationsCount);
    }

    [Fact]
    public void MinimumSaltSize_DefaultValue_IsSixteen() {
        // arrange & act
        var sut = new RijndaelCryptoOptions();

        // assert
        Assert.Equal(16, sut.MinimumSaltSize);
    }

    [Fact]
    public void MaximumSaltSize_DefaultValue_Is128() {
        // arrange & act
        var sut = new RijndaelCryptoOptions();

        // assert
        Assert.Equal(128, sut.MaximumSaltSize);
    }

    [Fact]
    public void KeyIterationsCount_WhenSetToValidValue_StoresValue() {
        // arrange
        var sut = new RijndaelCryptoOptions();

        // act
        sut.KeyIterationsCount = 10;

        // assert
        Assert.Equal(10, sut.KeyIterationsCount);
    }

    [Fact]
    public void KeyIterationsCount_WhenSetToZero_ThrowsArgumentException() {
        // arrange
        var sut = new RijndaelCryptoOptions();

        // act & assert
        Assert.ThrowsAny<ArgumentException>(() => sut.KeyIterationsCount = 0);
    }

    [Fact]
    public void KeyIterationsCount_WhenSetToNegative_ThrowsArgumentException() {
        // arrange
        var sut = new RijndaelCryptoOptions();

        // act & assert
        Assert.ThrowsAny<ArgumentException>(() => sut.KeyIterationsCount = -1);
    }

    [Fact]
    public void MinimumSaltSize_WhenSetToValidValue_StoresValue() {
        // arrange
        var sut = new RijndaelCryptoOptions();

        // act
        sut.MinimumSaltSize = 8;

        // assert
        Assert.Equal(8, sut.MinimumSaltSize);
    }

    [Fact]
    public void MinimumSaltSize_WhenSetBelowMinimum_ThrowsArgumentOutOfRangeException() {
        // arrange
        var sut = new RijndaelCryptoOptions();

        // act & assert — minimum allowed is 4
        Assert.ThrowsAny<ArgumentException>(() => sut.MinimumSaltSize = 3);
    }

    [Fact]
    public void MinimumSaltSize_WhenSetAboveMaximum_ThrowsArgumentOutOfRangeException() {
        // arrange
        var sut = new RijndaelCryptoOptions();

        // act & assert — maximum allowed is 255
        Assert.ThrowsAny<ArgumentException>(() => sut.MinimumSaltSize = 256);
    }

    [Fact]
    public void MaximumSaltSize_WhenSetToValidValue_StoresValue() {
        // arrange
        var sut = new RijndaelCryptoOptions();

        // act
        sut.MaximumSaltSize = 200;

        // assert
        Assert.Equal(200, sut.MaximumSaltSize);
    }

    [Fact]
    public void MaximumSaltSize_WhenSetBelowMinimum_ThrowsArgumentOutOfRangeException() {
        // arrange
        var sut = new RijndaelCryptoOptions();

        // act & assert
        Assert.ThrowsAny<ArgumentException>(() => sut.MaximumSaltSize = 1);
    }

    [Fact]
    public void MaximumSaltSize_WhenSetAboveMaximum_ThrowsArgumentOutOfRangeException() {
        // arrange
        var sut = new RijndaelCryptoOptions();

        // act & assert
        Assert.ThrowsAny<ArgumentException>(() => sut.MaximumSaltSize = 300);
    }

    [Fact]
    public void Passphrase_DefaultValue_IsNotEmpty() {
        // arrange & act
        var sut = new RijndaelCryptoOptions();

        // assert
        Assert.False(string.IsNullOrWhiteSpace(sut.Passphrase));
    }

    [Fact]
    public void Iv_DefaultValue_IsNotEmpty() {
        // arrange & act
        var sut = new RijndaelCryptoOptions();

        // assert
        Assert.False(string.IsNullOrWhiteSpace(sut.Iv));
    }

    [Fact]
    public void Salt_DefaultValue_IsNotEmpty() {
        // arrange & act
        var sut = new RijndaelCryptoOptions();

        // assert
        Assert.False(string.IsNullOrWhiteSpace(sut.Salt));
    }
}
