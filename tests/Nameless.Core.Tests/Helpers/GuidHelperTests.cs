using Nameless.Helpers;

namespace Nameless;

public class GuidHelperTests {
    // ─── Encode ──────────────────────────────────────────────────────────────

    [Fact]
    public void Encode_GuidEmpty_Returns22CharString() {
        // act
        var result = GuidHelper.Encode(Guid.Empty);

        // assert
        Assert.Equal(22, result.Length);
    }

    [Fact]
    public void Encode_AnyGuid_Returns22CharString() {
        // act
        var result = GuidHelper.Encode(Guid.NewGuid());

        // assert
        Assert.Equal(22, result.Length);
    }

    [Fact]
    public void Encode_Result_ContainsNoForwardSlash() {
        // arrange
        var encoded = GuidHelper.Encode(Guid.NewGuid());

        // assert
        Assert.DoesNotContain("/", encoded);
    }

    [Fact]
    public void Encode_Result_ContainsNoPlusSign() {
        // arrange
        var encoded = GuidHelper.Encode(Guid.NewGuid());

        // assert
        Assert.DoesNotContain("+", encoded);
    }

    // ─── Decode ──────────────────────────────────────────────────────────────

    [Fact]
    public void Decode_AfterEncode_RoundTripsToOriginalGuid() {
        // arrange
        var original = Guid.NewGuid();

        // act
        var encoded = GuidHelper.Encode(original);
        var decoded = GuidHelper.Decode(encoded);

        // assert
        Assert.Equal(original, decoded);
    }

    [Fact]
    public void Decode_EmptyGuid_RoundTripsCorrectly() {
        // arrange
        var original = Guid.Empty;

        // act
        var encoded = GuidHelper.Encode(original);
        var decoded = GuidHelper.Decode(encoded);

        // assert
        Assert.Equal(original, decoded);
    }

    // ─── Different guids produce different encodings ──────────────────────────

    [Fact]
    public void Encode_DifferentGuids_ProduceDifferentEncodings() {
        // arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();

        // act
        var encoded1 = GuidHelper.Encode(guid1);
        var encoded2 = GuidHelper.Encode(guid2);

        // assert
        Assert.NotEqual(encoded1, encoded2);
    }

    [Fact]
    public void Encode_AndDecode_GuidWithSlashAndPlusBytes_RoundTrips() {
        // 0xFB bytes produce '+' and '/' in raw base64, which Encode replaces with '_' and '-'.
        // Decode must reverse both substitutions, exercising the HYPHEN and UNDERSCORE branches.
        var original = new Guid("fbfbfbfb-fbfb-fbfb-fbfb-fbfbfbfbfbfb");

        var encoded = GuidHelper.Encode(original);
        var decoded = GuidHelper.Decode(encoded);

        Assert.Multiple(() => {
            Assert.Contains('_', encoded);
            Assert.Contains('-', encoded);
            Assert.Equal(original, decoded);
        });
    }

    [Fact]
    public void Decode_WithInvalidBase64_ThrowsInvalidOperationException() {
        // 22 chars required; '!' is not a valid base64 char and will cause TryFromBase64Chars to fail
        Assert.Throws<InvalidOperationException>(() => GuidHelper.Decode("AAAAAAAAAAAAAAAAAAAAA!"));
    }
}
