using Nameless.Helpers;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Testing.Tools;

[UnitTest]
public class ResourcesHelperTests {
    [Fact]
    public void WhenCreatingCopy_ThenReturnsFilePathToTemporaryFile() {
        // arrange
        var assemblyDirectoryPath = typeof(ResourcesHelperTests).Assembly.GetDirectoryPath();
        var temporaryRelativeFilePath =
            PathHelper.Normalize($@"{ResourcesHelper.TEMPORARY_DIRECTORY_NAME}\Samples\Sample.txt");
        var relativeFilePath = PathHelper.Normalize(path: @"Samples\Sample.txt");
        var expected = Path.Combine(assemblyDirectoryPath, temporaryRelativeFilePath);

        // act
        var actual = ResourcesHelper.CreateCopy(relativeFilePath);


        // assert
        Assert.Multiple(() => {
            Assert.EndsWith(temporaryRelativeFilePath, actual);
            Assert.Equal(expected, actual);
            Assert.True(File.Exists(actual));
        });
    }

    [Fact]
    public void WhenCreatingCopy_WithANewName_ThenReturnsFilePathToTemporaryFile() {
        // arrange
        const string NewFileName = "NewSample.txt";
        var assemblyDirectoryPath = typeof(ResourcesHelperTests).Assembly.GetDirectoryPath();
        var temporaryRelativeFilePath =
            PathHelper.Normalize($@"{ResourcesHelper.TEMPORARY_DIRECTORY_NAME}\Samples\{NewFileName}");
        var relativeFilePath = PathHelper.Normalize(path: @"Samples\Sample.txt");
        var expected = Path.Combine(assemblyDirectoryPath, temporaryRelativeFilePath);

        // act
        var actual = ResourcesHelper.CreateCopy(relativeFilePath, NewFileName);


        // assert
        Assert.Multiple(() => {
            Assert.EndsWith(temporaryRelativeFilePath, actual);
            Assert.Equal(expected, actual);
            Assert.True(File.Exists(actual));
        });
    }
}