using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nameless.Security.Cryptography;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Security.Cryptography;

[UnitTest]
public class RijndaelCryptoTests {
    // AES CBC transforms are stateful: after FlushFinalBlock the transform cannot
    // be reused. We create a fresh RijndaelCrypto instance for each operation so
    // the encryptor/decryptor state is always clean. Both instances are built from
    // the same options, which guarantees they derive identical keys and IVs.
    private static (RijndaelCrypto Encryptor, RijndaelCrypto Decryptor) CreateSutPair(
        Action<RijndaelCryptoOptions>? configure = null) {
        var opts = new RijndaelCryptoOptions {
            // KeySize.None (0) is defined in the enum, so Enum.IsDefined returns true
            // and Pbkdf2 would derive 0 bytes — always set an explicit valid key size.
            KeySize = KeySize.Large
        };
        configure?.Invoke(opts);

        var encryptor = Build(opts);
        var decryptor = Build(opts);
        return (encryptor, decryptor);

        static RijndaelCrypto Build(RijndaelCryptoOptions options) {
            var logger = new Mock<ILogger<RijndaelCrypto>>().Object;
            return new RijndaelCrypto(Options.Create(options), logger);
        }
    }

    [Fact]
    public void EncryptThenDecrypt_WithDefaultOptions_RoundTrips() {
        // arrange
        const string PlainText = "Hello, Nameless!";
        var originalBytes = Encoding.UTF8.GetBytes(PlainText);
        var (encSut, decSut) = CreateSutPair();

        // act
        byte[] encrypted;
        using (encSut)
        using (var encryptStream = new MemoryStream(originalBytes)) {
            encrypted = encSut.Encrypt(encryptStream);
        }

        byte[] decrypted;
        using (decSut)
        using (var decryptStream = new MemoryStream(encrypted)) {
            decrypted = decSut.Decrypt(decryptStream);
        }

        // assert
        Assert.Equal(originalBytes, decrypted);
    }

    [Fact]
    public void EncryptThenDecrypt_WithKeySizeLow_RoundTrips() {
        // arrange
        const string PlainText = "Low key size test";
        var originalBytes = Encoding.UTF8.GetBytes(PlainText);
        var (encSut, decSut) = CreateSutPair(opts => opts.KeySize = KeySize.Low);

        // act
        byte[] encrypted;
        using (encSut)
        using (var encryptStream = new MemoryStream(originalBytes)) {
            encrypted = encSut.Encrypt(encryptStream);
        }

        byte[] decrypted;
        using (decSut)
        using (var decryptStream = new MemoryStream(encrypted)) {
            decrypted = decSut.Decrypt(decryptStream);
        }

        // assert
        Assert.Equal(originalBytes, decrypted);
    }

    [Fact]
    public void EncryptThenDecrypt_WithKeySizeMedium_RoundTrips() {
        // arrange
        const string PlainText = "Medium key size test";
        var originalBytes = Encoding.UTF8.GetBytes(PlainText);
        var (encSut, decSut) = CreateSutPair(opts => opts.KeySize = KeySize.Medium);

        // act
        byte[] encrypted;
        using (encSut)
        using (var encryptStream = new MemoryStream(originalBytes)) {
            encrypted = encSut.Encrypt(encryptStream);
        }

        byte[] decrypted;
        using (decSut)
        using (var decryptStream = new MemoryStream(encrypted)) {
            decrypted = decSut.Decrypt(decryptStream);
        }

        // assert
        Assert.Equal(originalBytes, decrypted);
    }

    [Fact]
    public void EncryptThenDecrypt_WithKeySizeLarge_RoundTrips() {
        // arrange
        const string PlainText = "Large key size test";
        var originalBytes = Encoding.UTF8.GetBytes(PlainText);
        var (encSut, decSut) = CreateSutPair(opts => opts.KeySize = KeySize.Large);

        // act
        byte[] encrypted;
        using (encSut)
        using (var encryptStream = new MemoryStream(originalBytes)) {
            encrypted = encSut.Encrypt(encryptStream);
        }

        byte[] decrypted;
        using (decSut)
        using (var decryptStream = new MemoryStream(encrypted)) {
            decrypted = decSut.Decrypt(decryptStream);
        }

        // assert
        Assert.Equal(originalBytes, decrypted);
    }

    [Fact]
    public void Encrypt_EmptyStream_ThrowsInvalidOperationException() {
        // arrange
        var (encSut, _) = CreateSutPair();
        using var emptyStream = new MemoryStream();

        // act
        var exception = Record.Exception(() => encSut.Encrypt(emptyStream));

        // assert
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async Task EncryptDecrypt_ConcurrentCalls_DoNotCorruptOutput() {
        // arrange
        const int TaskCount = 10;

        // Each task gets its own pair of instances: AES CBC transforms are not
        // reusable after FlushFinalBlock, so concurrent reuse of a shared instance
        // would corrupt state. Thread-safety within a single Encrypt or Decrypt
        // call is guaranteed by the internal Lock, but back-to-back calls still
        // require a fresh transform — hence one pair per task.
        var tasks = Enumerable.Range(0, TaskCount).Select(idx => Task.Run(() => {
            var plainText = $"Concurrent message number {idx}";
            var originalBytes = Encoding.UTF8.GetBytes(plainText);
            var (encSut, decSut) = CreateSutPair();

            byte[] encrypted;
            using (encSut)
            using (var encryptStream = new MemoryStream(originalBytes)) {
                encrypted = encSut.Encrypt(encryptStream);
            }

            byte[] decrypted;
            using (decSut)
            using (var decryptStream = new MemoryStream(encrypted)) {
                decrypted = decSut.Decrypt(decryptStream);
            }

            return (Original: originalBytes, Decrypted: decrypted);
        })).ToArray();

        // act
        var results = await Task.WhenAll(tasks);

        // assert
        Assert.All(results, r => Assert.Equal(r.Original, r.Decrypted));
    }
}
