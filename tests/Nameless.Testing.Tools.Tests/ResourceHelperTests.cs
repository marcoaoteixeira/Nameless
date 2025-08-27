using Nameless.Testing.Tools.Attributes;

namespace Nameless.Testing.Tools;

[UnitTest]
public class ResourceHelperTests {
    [Fact]
    public void WhenCreatingCopy_ThenReturnsFilePathToTemporaryFile() {
        // arrange
        var assemblyDirectoryPath = typeof(ResourceHelperTests).Assembly.GetDirectoryPath();
        const string TemporaryRelativeFilePath = @"Temporary\Samples\Sample.txt";
        const string RelativeFilePath = @"\Samples\Sample.txt";
        var expected = Path.Combine(assemblyDirectoryPath, TemporaryRelativeFilePath);

        // act
        var actual = ResourceHelper.CreateCopy(RelativeFilePath);


        // assert
        Assert.Multiple(() => {
            Assert.EndsWith(TemporaryRelativeFilePath, actual);
            Assert.Equal(expected, actual);
            Assert.True(File.Exists(expected));
        });
    }

    [Fact]
    public void WhenCreatingCopy_WithANewName_ThenReturnsFilePathToTemporaryFile() {
        // arrange
        const string NewFileName = "NewSample.txt";
        var assemblyDirectoryPath = typeof(ResourceHelperTests).Assembly.GetDirectoryPath();
        const string TemporaryRelativeFilePath = $@"Temporary\Samples\{NewFileName}";
        const string RelativeFilePath = @"\Samples\Sample.txt";
        var expected = Path.Combine(assemblyDirectoryPath, TemporaryRelativeFilePath);

        // act
        var actual = ResourceHelper.CreateCopy(RelativeFilePath, "NewSample");


        // assert
        Assert.Multiple(() => {
            Assert.EndsWith(TemporaryRelativeFilePath, actual);
            Assert.Equal(expected, actual);
            Assert.True(File.Exists(expected));
        });
    }
}
