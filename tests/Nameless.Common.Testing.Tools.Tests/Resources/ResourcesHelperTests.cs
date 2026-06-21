using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Resources;

namespace Nameless.Common.Testing.Tools.Resources;

[UnitTest]
public class ResourcesHelperTests
{
    private const string LOREM_IPSUM_RESOURCE = "lorem_ipsum.txt";

    [Fact]
    public void WhenGetResource_WithValidRelativePath_ThenShouldReturnResource()
    {
        // arrange

        // act
        var actual = ResourcesHelper.GetResource(LOREM_IPSUM_RESOURCE, createCopy: false);

        // assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(actual);
            Assert.NotEmpty(actual.Path);
            Assert.True(File.Exists(actual.Path));
            Assert.False(actual.DeleteOnDispose);

            actual.Dispose();
        });
    }

    [Fact]
    public void WhenGetResource_WithValidRelativePath_WhenIsACopy_ThenShouldDeleteOnDispose()
    {
        // arrange

        // act
        var actual = ResourcesHelper.GetResource(LOREM_IPSUM_RESOURCE, createCopy: true);

        // assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(actual);

            Assert.NotEmpty(actual.Path);

            Assert.True(File.Exists(actual.Path));

            Assert.StartsWith(Path.GetTempPath(), actual.Path);

            Assert.True(actual.DeleteOnDispose);

            actual.Dispose();

            Assert.False(File.Exists(actual.Path));
        });
    }

    [Fact]
    public void WhenOpen_FromResource_WithValidRelativePath_ThenReturnsResourceStream()
    {
        // arrange
        var sut = ResourcesHelper.GetResource(LOREM_IPSUM_RESOURCE);

        // act
        var stream = sut.Open();

        // assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(stream);
            Assert.NotEqual(0, stream.Length);

            stream.Dispose();
            sut.Dispose();
        });
    }

    [Fact]
    public void WhenGettingContent_FromResource_WithValidRelativePath_ThenReturnsResourceContent()
    {
        // arrange
        var sut = ResourcesHelper.GetResource(LOREM_IPSUM_RESOURCE);

        // act
        var content = sut.GetContent();

        // assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(content);
            Assert.NotEmpty(content);
            Assert.Contains("Nullam consequat erat efficitur", content);

            sut.Dispose();
        });
    }

    [Fact]
    public void WhenGettingResource_WhenNotIsCopy_WhenDispose_ThenResourceShouldNotBeDeleted()
    {
        // arrange
        var sut = ResourcesHelper.GetResource(LOREM_IPSUM_RESOURCE, createCopy: false);

        // act
        var content = sut.GetContent();

        // assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(sut);

            Assert.NotEmpty(sut.Path);

            Assert.True(File.Exists(sut.Path));

            Assert.False(sut.Path.StartsWith(Path.GetTempPath()));

            Assert.False(sut.DeleteOnDispose);

            Assert.NotNull(content);

            Assert.NotEmpty(content);

            Assert.Contains("Nullam consequat erat efficitur", content);

            sut.Dispose();

            Assert.True(File.Exists(sut.Path));
        });
    }

    [Fact]
    public void WhenGettingResource_WhenRelativePathOutsideRootDirectory_ThenThrowsException()
    {
        // arrange

        // act
        var actual = Record.Exception(() => ResourcesHelper.GetResource("../../../../../../something.dat", createCopy: false));

        // assert
        Assert.Multiple(() =>
        {
            Assert.IsType<InvalidOperationException>(actual);
            Assert.Contains("outside the root", actual.Message);
        });
    }

    [Fact]
    public void WhenGettingResource_WhenIsNotCopy_WhenFileDoesNotExist_ThenThrowsException()
    {
        // arrange

        // act
        var actual = Record.Exception(() => ResourcesHelper.GetResource("something.dat", createCopy: false));

        // assert
        Assert.IsType<FileNotFoundException>(actual);
    }
}