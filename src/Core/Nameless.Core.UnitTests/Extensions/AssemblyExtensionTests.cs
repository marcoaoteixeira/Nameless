using System.Reflection;
using Moq;

namespace Nameless.Core.UnitTests.Extensions {
    public class AssemblyExtensionTests {

        [Test]
        public void Get_Assembly_Directory_Path_In_FileSystem() {
            // arrange
            var directory = "C:\\This\\Is\\A\\Test";
            var assembly = new Mock<Assembly>();
            assembly
                .Setup(_ => _.Location)
                .Returns($"{directory}\\Assembly.dll");

            // act
            var path = AssemblyExtension.GetDirectoryPath(assembly.Object);

            // assert
            Assert.That(path, Is.Not.Null);
            Assert.That(path, Is.EqualTo(directory));
        }
    }
}
