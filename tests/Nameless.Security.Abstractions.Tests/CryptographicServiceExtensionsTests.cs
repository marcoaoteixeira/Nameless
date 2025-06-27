using System.Text;
using Moq;
using Nameless.Security.Crypto;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Security;
public class CryptographicServiceExtensionsTests {
    [Fact]
    public void WhenEncryptingStringValue_ThenReturnsEncryptedValue() {
        // arrange
        const string Expected = "This is a Test";
        var cryptographicService = new CryptographicServiceMocker().WithEncrypt("This is a Test")
                                                                   .Build();

        // act
        var actual = cryptographicService.Encrypt(Expected);

        // assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void WhenDecryptingStringValue_ThenReturnsDecryptedValue() {
        // arrange
        const string Expected = "This is a Test";
        var cryptographicService = new CryptographicServiceMocker().WithDecrypt("This is a Test")
                                                                   .Build();

        // act
        var actual = cryptographicService.Decrypt(Expected);

        // assert
        Assert.Equal(Expected, actual);
    }
}

public class CryptographicServiceMocker : Mocker<ICryptographicService> {
    public CryptographicServiceMocker WithEncrypt(string result) {
        return WithEncrypt(Encoding.UTF8.GetBytes(result));
    }

    public CryptographicServiceMocker WithEncrypt(byte[] result) {
        MockInstance
           .Setup(mock => mock.Encrypt(It.IsAny<Stream>()))
           .Returns(result);

        return this;
    }

    public CryptographicServiceMocker WithDecrypt(string result) {
        return WithDecrypt(Encoding.UTF8.GetBytes(result));
    }

    public CryptographicServiceMocker WithDecrypt(byte[] result) {
        MockInstance
           .Setup(mock => mock.Decrypt(It.IsAny<Stream>()))
           .Returns(result);

        return this;
    }
}
