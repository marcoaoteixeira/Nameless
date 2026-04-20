using System.Xml.Linq;

namespace Nameless;

public class XmlExtensionsTests {
    // --- XContainerExtensions.HasElement(name) ---

    [Fact]
    public void HasElement_WhenElementExists_ReturnsTrue() {
        // arrange
        var doc = XElement.Parse("<root><child/></root>");

        // act & assert
        Assert.True(doc.HasElement("child"));
    }

    [Fact]
    public void HasElement_WhenElementMissing_ReturnsFalse() {
        // arrange
        var doc = XElement.Parse("<root><child/></root>");

        // act & assert
        Assert.False(doc.HasElement("nothere"));
    }

    // --- XContainerExtensions.HasElement(name, attribute, value) ---

    [Fact]
    public void HasElement_WithAttributeMatch_ReturnsTrue() {
        // arrange
        var doc = XElement.Parse("<root><item id=\"42\"/></root>");

        // act & assert
        Assert.True(doc.HasElement("item", "id", "42"));
    }

    [Fact]
    public void HasElement_WithAttributeNoMatch_ReturnsFalse() {
        // arrange
        var doc = XElement.Parse("<root><item id=\"99\"/></root>");

        // act & assert
        Assert.False(doc.HasElement("item", "id", "42"));
    }

    // --- XElementExtensions.HasAttribute ---

    [Fact]
    public void HasAttribute_WhenAttributeExists_ReturnsTrue() {
        // arrange
        var element = XElement.Parse("<item id=\"1\" name=\"test\"/>");

        // act & assert
        Assert.True(element.HasAttribute("id"));
    }

    [Fact]
    public void HasAttribute_WhenAttributeMissing_ReturnsFalse() {
        // arrange
        var element = XElement.Parse("<item id=\"1\"/>");

        // act & assert
        Assert.False(element.HasAttribute("missing"));
    }
}
