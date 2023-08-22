namespace Nameless.Core.UnitTests.Extensions {
    public class ArrayExtensionTests {

        [Test]
        public void Get_An_Item_By_Index_Without_Fuss() {
            // arrange
            var array = new[] { 1, 2, 3 };

            // act
            ArrayExtension.TryElementAt(array, 1, out var item);

            // assert
            Assert.That(item, Is.EqualTo(2));
        }

        [Test]
        public void If_The_Item_Exists_Returns_True_And_Output() {
            // arrange
            var array = new[] { 1, 2, 3 };

            // act
            var exists = ArrayExtension.TryElementAt(array, 1, out var item);

            // assert
            Assert.That(item, Is.EqualTo(2));
            Assert.That(exists, Is.True);
        }

        [Test]
        public void If_The_Item_Does_Not_Exists_Returns_False_And_Output_Default() {
            // arrange
            var array = new[] { 1, 2, 3 };

            // act
            var exists = ArrayExtension.TryElementAt(array, 5, out var item);

            // assert
            Assert.That(item, Is.EqualTo(default(int)));
            Assert.That(exists, Is.False);
        }

        [Test]
        public void If_Array_Is_Null_Returns_False_And_Output_Default() {
            // arrange
            int[] array = default!;

            // act
            var exists = ArrayExtension.TryElementAt(array, 5, out var item);

            // assert
            Assert.That(item, Is.EqualTo(default(int)));
            Assert.That(exists, Is.False);
        }
    }
}
