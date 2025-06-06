using System.Collections;

namespace Nameless.Utils;

public class DeepCopyTests {
    [Fact]
    public void WhenCloning_ThenReturnsDifferentReferenceWithNewObject() {
        // arrange
        var student = new Student { ID = 1, Name = "John Doe", Age = 25 };

        // act
        var newStudent = DeepCopy.Clone(student);

        // assert
        Assert.NotSame(newStudent, student);
    }

    [Fact]
    public void WhenCloningRuntimeType_ThrowsInvalidOperationException() {
        // arrange
        var value = typeof(IList);

        // act && assert
        Assert.Throws<InvalidOperationException>(() => DeepCopy.Clone(value));
    }

    public record Student : EntityBase {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public abstract record EntityBase {
        public int ID { get; set; }
    }
}