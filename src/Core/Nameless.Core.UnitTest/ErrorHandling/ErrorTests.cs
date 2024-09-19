namespace Nameless.ErrorHandling;
public class ErrorTests {
    [Test]
    public void When_Creating_Error_Instance_Then_Code_Or_Problems_Must_Be_Non_Null() {
        // arrange
        const string code = "Error Code";
        var problems = new[] { "Message 1", "Message 2" };

        // act
        var error = new Error(code, problems);

        // assert
        Assert.Multiple(() => {
            Assert.That(error.Code, Is.EqualTo(code));
            Assert.That(error.Problems, Is.EquivalentTo(problems));
        });
    }

    [Test]
    public void When_Creating_Error_With_Null_Code_Then_Throw_ArgumentNullException() {
        // arrange
        string? code = null;
        var problems = new[] { "Message 1", "Message 2" };

        // act & assert
        Assert.Throws<ArgumentNullException>(() => _ = new Error(code!, problems));
    }

    [Test]
    public void When_Creating_Error_With_Null_Problems_Then_Throw_ArgumentNullException() {
        // arrange
        const string code = "Error Code";
        string[]? problems = null;

        // act & assert
        Assert.Throws<ArgumentNullException>(() => _ = new Error(code, problems!));
    }

    [Test]
    public void When_Creating_Error_With_Empty_Code_Then_Return_Instance() {
        // arrange
        const string code = "";
        var problems = new[] { "Message 1", "Message 2" };

        // act
        var actual = new Error(code, problems);

        // assert
        Assert.Multiple(() => {
            Assert.That(actual.Code, Is.EqualTo(code));
            Assert.That(actual.Problems, Is.EquivalentTo(problems));
        });
    }

    [Test]
    public void When_Creating_Error_With_Empty_Problems_Then_Return_Instance() {
        // arrange
        const string code = "Error Code";
        var problems = Array.Empty<string>();

        // act
        var actual = new Error(code, problems);

        // assert
        Assert.Multiple(() => {
            Assert.That(actual.Code, Is.EqualTo(code));
            Assert.That(actual.Problems, Is.EquivalentTo(problems));
        });
    }
}
