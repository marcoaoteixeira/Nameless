namespace Nameless.UnitTests {
    public class ForNullTests {
        [Test]
        public void Throws_ArgumentNullException_On_Object_Null() {
            Assert.Throws<ArgumentNullException>(() => {
                object? value = null;

                Prevent.Against.Null(value, nameof(value));
            });
        }

        [Test]
        public void Does_Not_Throws_On_Object_Not_Null() {
            Assert.DoesNotThrow(() => {
                object? value = "123";

                Prevent.Against.Null(value, nameof(value));
            });
        }

        [Test]
        public void Does_Not_Throws_On_Object_Not_Null_ValueType() {
            Assert.DoesNotThrow(() => {
                object? value = 123;

                Prevent.Against.Null(value, nameof(value));
            });
        }
    }
}