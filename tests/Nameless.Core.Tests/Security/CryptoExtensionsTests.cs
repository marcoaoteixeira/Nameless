using Moq;
using Nameless.Security.Cryptography;

namespace Nameless.Security;

public class CryptoExtensionsTests {
    // --- Encrypt ---

    [Fact]
    public void Encrypt_CallsEncryptOnCrypto_WithCorrectBytes() {
        // arrange
        var input = "hello";
        var expected = "ENCRYPTED";
        var mock = new Mock<ICrypto>();
        mock.Setup(c => c.Encrypt(It.IsAny<Stream>()))
            .Returns(CoreDefaults.Encoding.GetBytes(expected));

        // act
        var result = mock.Object.Encrypt(input);

        // assert
        Assert.Equal(expected, result);
    }

    // --- Decrypt ---

    [Fact]
    public void Decrypt_CallsDecryptOnCrypto_WithCorrectBytes() {
        // arrange
        var input = "encrypted";
        var expected = "DECRYPTED";
        var mock = new Mock<ICrypto>();
        mock.Setup(c => c.Decrypt(It.IsAny<Stream>()))
            .Returns(CoreDefaults.Encoding.GetBytes(expected));

        // act
        var result = mock.Object.Decrypt(input);

        // assert
        Assert.Equal(expected, result);
    }
}
