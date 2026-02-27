using Nameless.Testing.Tools.Attributes;

namespace Nameless.Testing.Tools.Resources;

[UnitTest]
public class ResourcesHelperTests {
    private const string RELATIVE_PATH = "Samples/Sample.txt";

    [Fact]
    public void WhenGetResource_WithValidRelativePath_ThenReturnsResource() {
        // arrange

        // act
        var actual = ResourcesHelper.GetResource(RELATIVE_PATH, createCopy: false);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);
            Assert.NotEmpty(actual.Path);
            Assert.False(actual.DeleteOnDispose);

            actual.Dispose();
        });
    }

    [Fact]
    public void WhenGetResource_WhenOpenStream_WithValidRelativePath_ThenReturnsResourceStream() {
        // arrange
        var resource = ResourcesHelper.GetResource(RELATIVE_PATH, createCopy: false);

        // act
        var actual = resource.Open();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);
            Assert.NotEqual(0, actual.Length);
            
            resource.Dispose();
        });
    }

    [Fact]
    public void WhenGetResource_WhenGetContent_WithValidRelativePath_ThenReturnsResourceContent() {
        // arrange
        var resource = ResourcesHelper.GetResource(RELATIVE_PATH, createCopy: false);

        // act
        var actual = resource.GetContent();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.Contains("testing purposes", actual);

            resource.Dispose();
        });
    }

    [Fact]
    public void WhenGetResource_WhenIsDisposable_WithValidRelativePath_ThenResourceShouldBeDeletedOnDispose() {
        // arrange
        var resource = ResourcesHelper.GetResource(RELATIVE_PATH, createCopy: true);

        // act
        var actual = resource.Open();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);
            Assert.NotEqual(0, actual.Length);
            Assert.True(File.Exists(resource.Path));

            resource.Dispose();

            Assert.False(File.Exists(resource.Path));
        });
    }

    [Fact]
    public async Task WhenGetResource_WhenIsDisposable_WithValidRelativePath_ThenResourceShouldBeDeletedOnDisposeAsync() {
        // arrange
        var resource = ResourcesHelper.GetResource(RELATIVE_PATH, createCopy: true);

        // act
        var actual = resource.Open();

        // assert
        await Assert.MultipleAsync(async () => {
            Assert.NotNull(actual);
            Assert.NotEqual(0, actual.Length);
            Assert.True(File.Exists(resource.Path));

            await resource.DisposeAsync();

            Assert.False(File.Exists(resource.Path));
        });
    }

    [Fact]
    public void WhenGetResource_WhenMultipleCallsToDispose_ThenDoNotThrowException() {
        // arrange
        var resource = ResourcesHelper.GetResource(RELATIVE_PATH, createCopy: false);

        // act
        var actual = Record.Exception(() => {
            resource.Dispose();
            resource.Dispose();
            resource.Dispose();
        });

        // assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task WhenGetResource_WhenMultipleCallsToDisposeAsync_ThenDoNotThrowException() {
        // arrange
        var resource = ResourcesHelper.GetResource(RELATIVE_PATH, createCopy: false);

        // act
        var actual = await Record.ExceptionAsync(async () => {
            await resource.DisposeAsync();
            await resource.DisposeAsync();
            await resource.DisposeAsync();
        });

        // assert
        Assert.Null(actual);
    }
}