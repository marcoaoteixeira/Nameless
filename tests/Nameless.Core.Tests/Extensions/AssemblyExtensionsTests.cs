using System.Reflection;
using Moq;
using Nameless.Helpers;

namespace Nameless;

public class AssemblyExtensionsTests {
    [Fact]
    public void Get_Assembly_Directory_Path_In_FileSystem() {
        // arrange
        var pathSeparator = OperatingSystem.IsWindows() ? '\\' : '/';
        var directory = string.Join(pathSeparator, "C:", "This", "Is", "A", "Test");
        var expected = PathHelper.Normalize(directory);
        var assembly = new Mock<Assembly>();
        assembly.Setup(mock => mock.Location)
                .Returns($"{directory}{pathSeparator}Assembly.dll");

        // act
        var path = assembly.Object.GetDirectoryPath();

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(path);
            Assert.Equal(expected: expected, path);
        });
    }
}