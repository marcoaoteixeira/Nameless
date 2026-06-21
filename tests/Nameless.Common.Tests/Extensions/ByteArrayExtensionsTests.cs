namespace Nameless.Extensions;

public class ByteArrayExtensionsTests {
    // ─── ToHexString ────────────────────────────────────────────────────────

    [Fact]
    public void ToHexString_WithKnownBytes_ReturnsExpectedHex() {
        // arrange
        var bytes = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };

        // act
        var hex = bytes.ToHexString();

        // assert
        Assert.Equal("DEADBEEF", hex);
    }

    [Fact]
    public void ToHexString_WithEmptyArray_ReturnsEmptyString() {
        // arrange
        var bytes = Array.Empty<byte>();

        // act
        var hex = bytes.ToHexString();

        // assert
        Assert.Equal(string.Empty, hex);
    }

    [Fact]
    public void ToHexString_WithAllZeroBytes_ReturnsZeroHex() {
        // arrange
        var bytes = new byte[] { 0x00, 0x00 };

        // act
        var hex = bytes.ToHexString();

        // assert
        Assert.Equal("0000", hex);
    }

    // ─── ToBase64String ─────────────────────────────────────────────────────

    [Fact]
    public void ToBase64String_WithKnownBytes_ReturnsBase64() {
        // arrange
        var bytes = "hello"u8.ToArray();

        // act
        var base64 = bytes.ToBase64String();

        // assert
        Assert.Equal(Convert.ToBase64String(bytes), base64);
    }

    [Fact]
    public void ToBase64String_WithEmptyArray_ReturnsEmptyBase64() {
        // arrange
        var bytes = Array.Empty<byte>();

        // act
        var base64 = bytes.ToBase64String();

        // assert
        Assert.Equal(string.Empty, base64);
    }

    [Fact]
    public void ToBase64String_RoundTrips_WithOriginalBytes() {
        // arrange
        var original = "test data"u8.ToArray();

        // act
        var base64 = original.ToBase64String();
        var roundTripped = Convert.FromBase64String(base64);

        // assert
        Assert.Equal(original, roundTripped);
    }
}
