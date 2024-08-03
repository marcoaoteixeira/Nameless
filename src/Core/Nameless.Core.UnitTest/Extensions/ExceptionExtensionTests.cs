using System.Runtime.InteropServices;
using System.Security;

namespace Nameless {
    public class ExceptionExtensionTests {
        // This test should also test for ThreadAbortException, but
        // since is not possible to instantiate a new ThreadAbortException
        // will skip it.
        [TestCase(typeof(FatalException))]
        [TestCase(typeof(StackOverflowException))]
        [TestCase(typeof(OutOfMemoryException))]
        [TestCase(typeof(AccessViolationException))]
        [TestCase(typeof(AppDomainUnloadedException))]
        [TestCase(typeof(SecurityException))]
        [TestCase(typeof(SEHException))]
        public void IsFatal_Returns_True_For_Specified_Exceptions(Type exceptionType) {
            // arrange
            var instance = Activator.CreateInstance(exceptionType);

            if (instance is not Exception exception) {
                Assert.Fail($"Exception {exceptionType.Name} must have a parameterless constructor.");
                return;
            }

            // act
            var actual = ExceptionExtension.IsFatal(exception);

            // assert
            Assert.That(actual, Is.True);
        }
    }
}
