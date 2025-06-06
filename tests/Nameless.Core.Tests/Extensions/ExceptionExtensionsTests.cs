using System.Runtime.InteropServices;
using System.Security;

namespace Nameless;

public class ExceptionExtensionsTests {
    // This test should also test for ThreadAbortException, but
    // since is not possible to instantiate a new ThreadAbortException
    // will skip it.
    [Theory]
    [InlineData(typeof(StackOverflowException))]
    [InlineData(typeof(OutOfMemoryException))]
    [InlineData(typeof(AccessViolationException))]
    [InlineData(typeof(AppDomainUnloadedException))]
    [InlineData(typeof(SecurityException))]
    [InlineData(typeof(SEHException))]
    public void IsFatal_Returns_True_For_Specified_Exceptions(Type exceptionType) {
        // arrange
        var instance = Activator.CreateInstance(exceptionType);

        if (instance is not Exception exception) {
            Assert.Fail($"Exception {exceptionType.Name} must have a parameterless constructor.");
            return;
        }

        // act
        var actual = exception.IsFatal();

        // assert
        Assert.True(actual);
    }
}