using Nameless.Fixtures;

namespace Nameless {
    public class ObjectExtensionTests {
        [Test]
        public void IsAnonymous_Should_Returns_True_If_Anonymous_Object_Instance() {
            // arrange
            var obj = new {
                Id = 1,
                Name = "John Wick"
            };

            // act
            var actual = ObjectExtension.IsAnonymous(obj);

            // assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void IsAnonymous_Should_Returns_False_If_Class_Instance() {
            // arrange
            var obj = new Student {
                Age = 50,
                Name = "John Wick"
            };

            // act
            var actual = ObjectExtension.IsAnonymous(obj);

            // assert
            Assert.That(actual, Is.False);
        }
    }
}
