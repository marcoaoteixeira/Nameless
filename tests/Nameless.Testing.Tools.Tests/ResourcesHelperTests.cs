using Nameless.Helpers;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;

namespace Nameless.Testing.Tools;

[UnitTest]
public class ResourcesHelperTests {
    [Fact]
    public void WhenCreatingCopy_ThenReturnsFilePathToTemporaryFile() {
        // arrange
        var relativeFilePath = PathHelper.Normalize(@"Samples\Sample.txt");
        var expected = Path.Combine(
            typeof(ResourcesHelperTests).Assembly.GetDirectoryPath(),
            ResourcesHelper.ROOT_DIRECTORY_NAME,
            ResourcesHelper.TEMPORARY_DIRECTORY_NAME,
            "Samples"
        );

        // act
        var actual = ResourcesHelper.CreateCopy(relativeFilePath);

        // assert
        Assert.Multiple(() => {
            Assert.StartsWith(expected, actual);
            Assert.True(File.Exists(actual));
        });
    }

    [Fact]
    public void WhenCreatingCopy_WithANewName_ThenReturnsFilePathToTemporaryFile() {
        // arrange
        const string NewFileName = "NewSample.txt";
        var relativeFilePath = PathHelper.Normalize(@"Samples\Sample.txt");
        var expected = Path.Combine(
            typeof(ResourcesHelperTests).Assembly.GetDirectoryPath(),
            ResourcesHelper.ROOT_DIRECTORY_NAME,
            ResourcesHelper.TEMPORARY_DIRECTORY_NAME,
            "Samples",
            "NewSample.txt"
        );

        // act
        var actual = ResourcesHelper.CreateCopy(relativeFilePath, NewFileName);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(expected, actual);
            Assert.True(File.Exists(actual));
        });
    }
}