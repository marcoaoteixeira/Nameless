using System.Reflection;
using Moq;
using Nameless.Helpers;

namespace Nameless;

public class AssemblyExtensionTests {

    [Test]
    public void Get_Assembly_Directory_Path_In_FileSystem() {
        // arrange
        var pathSeparator = OperatingSystem.IsWindows() ? '\\' : '/';
        var directory = string.Join(pathSeparator, "C:", "This", "Is", "A", "Test");
        var expected = PathHelper.Normalize(directory);
        var assembly = new Mock<Assembly>();
        assembly.Setup(mock => mock.Location)
                .Returns($"{directory}{pathSeparator}Assembly.dll");

        // act
        var path = AssemblyExtension.GetDirectoryPath(assembly.Object);

        // assert
        Assert.That(path, Is.Not.Null);
        Assert.That(path, Is.EqualTo(expected));
    }
}