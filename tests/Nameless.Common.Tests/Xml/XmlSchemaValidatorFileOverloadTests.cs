using Moq;

namespace Nameless.Xml;

[IntegrationTest]
public class XmlSchemaValidatorFileOverloadTests : IDisposable {
    private readonly string _root = SysPath.Combine(SysPath.GetTempPath(), $"xml-{Guid.NewGuid():N}");

    public XmlSchemaValidatorFileOverloadTests() {
        SysDirectory.CreateDirectory(_root);
    }

    public void Dispose() {
        SysDirectory.Delete(_root, recursive: true);
    }

    [Fact]
    public void Validate_WithFilePaths_PassesFileContentsToStreamOverload() {
        // arrange
        var xmlPath = SysPath.Combine(_root, "a.xml");
        var xsdPath = SysPath.Combine(_root, "a.xsd");
        SysFile.WriteAllText(xmlPath, "<root/>");
        SysFile.WriteAllText(xsdPath, "<schema/>");

        string? xmlRead = null;
        string? schemaRead = null;
        var mock = new Mock<IXmlSchemaValidator>();
        mock.Setup(v => v.Validate(It.IsAny<Stream>(), It.IsAny<Stream>()))
            .Returns<Stream, Stream>((xml, schema) => {
                xmlRead = new StreamReader(xml).ReadToEnd();
                schemaRead = new StreamReader(schema).ReadToEnd();
                return true;
            });

        // act
        var result = mock.Object.Validate(xmlPath, xsdPath);

        // assert
        Assert.Multiple(
            () => Assert.True(result),
            () => Assert.Equal("<root/>", xmlRead),
            () => Assert.Equal("<schema/>", schemaRead)
        );
    }

    [Fact]
    public void Validate_WithMissingFile_Throws() {
        // arrange
        var mock = new Mock<IXmlSchemaValidator>();

        // act & assert
        Assert.Throws<FileNotFoundException>(() => mock.Object.Validate(SysPath.Combine(_root, "none.xml"), SysPath.Combine(_root, "none.xsd")));
    }
}
