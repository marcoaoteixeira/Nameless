using Nameless.Helpers;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Testing.Tools;

[UnitTest]
public class ResourceHelperTests {
    [Fact]
    public void WhenCreatingCopy_ThenReturnsFilePathToTemporaryFile() {
        // arrange
        var assemblyDirectoryPath = typeof(ResourceHelperTests).Assembly.GetDirectoryPath();
        var temporaryRelativeFilePath = PathHelper.Normalize(@"Temporary\Samples\Sample.txt");
        var relativeFilePath = PathHelper.Normalize(@"\Samples\Sample.txt");
        var expected = Path.Combine(assemblyDirectoryPath, temporaryRelativeFilePath);

        // act
        var actual = ResourceHelper.CreateCopy(relativeFilePath);


        // assert
        Assert.Multiple(() => {
            Assert.EndsWith(temporaryRelativeFilePath, actual);
            Assert.Equal(expected, actual);
            Assert.True(File.Exists(expected));
        });
    }

    [Fact]
    public void WhenCreatingCopy_WithANewName_ThenReturnsFilePathToTemporaryFile() {
        // arrange
        const string NewFileName = "NewSample.txt";
        var assemblyDirectoryPath = typeof(ResourceHelperTests).Assembly.GetDirectoryPath();
        var temporaryRelativeFilePath = PathHelper.Normalize($@"Temporary\Samples\{NewFileName}");
        var relativeFilePath = PathHelper.Normalize(@"\Samples\Sample.txt");
        var expected = Path.Combine(assemblyDirectoryPath, temporaryRelativeFilePath);

        // act
        var actual = ResourceHelper.CreateCopy(relativeFilePath, "NewSample");


        // assert
        Assert.Multiple(() => {
            Assert.EndsWith(temporaryRelativeFilePath, actual);
            Assert.Equal(expected, actual);
            Assert.True(File.Exists(expected));
        });
    }
}
