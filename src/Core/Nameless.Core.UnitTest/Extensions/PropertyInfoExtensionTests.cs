using Nameless.Fixtures;

namespace Nameless {
    public class PropertyInfoExtensionTests {
        [Test]
        public void GetDescription_Should_Return_Property_DescriptionAttribute_Value() {
            // arrange
            var instance = new PropertyAnnotatedClass();
            var expected = "Name of something";

            // act
            var actual = PropertyInfoExtension.GetDescription(instance.GetPropertyWithDescription());

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetDescription_Should_Return_Null_If_Property_Not_Annotated_With_DescriptionAttribute() {
            // arrange
            var instance = new PropertyAnnotatedClass();

            // act
            var actual = PropertyInfoExtension.GetDescription(instance.GetPropertyWithoutDescription());

            // assert
            Assert.That(actual, Is.Null);
        }
    }
}
