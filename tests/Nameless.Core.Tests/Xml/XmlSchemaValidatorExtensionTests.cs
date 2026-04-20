using Moq;
using Nameless.Xml;

namespace Nameless;

public class XmlSchemaValidatorExtensionTests {
    // --- Validate(byte[], byte[]) ---

    [Fact]
    public void Validate_WithBuffers_DelegatesToStreamOverload() {
        // arrange
        var xmlBuffer = "<root/>"u8.ToArray();
        var schemaBuffer = "<schema/>"u8.ToArray();

        var mock = new Mock<IXmlSchemaValidator>();
        mock.Setup(v => v.Validate(It.IsAny<Stream>(), It.IsAny<Stream>()))
            .Returns(true);

        // act
        var result = mock.Object.Validate(xmlBuffer, schemaBuffer);

        // assert
        Assert.True(result);
        mock.Verify(v => v.Validate(It.IsAny<Stream>(), It.IsAny<Stream>()), Times.Once);
    }

    [Fact]
    public void Validate_WhenStreamValidationReturnsFalse_ReturnsFalse() {
        // arrange
        var mock = new Mock<IXmlSchemaValidator>();
        mock.Setup(v => v.Validate(It.IsAny<Stream>(), It.IsAny<Stream>()))
            .Returns(false);

        // act
        var result = mock.Object.Validate("<root/>"u8.ToArray(), "<schema/>"u8.ToArray());

        // assert
        Assert.False(result);
    }
}
