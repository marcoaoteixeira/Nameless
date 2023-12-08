namespace Nameless.Core.UnitTest.Extensions {
    public class DictionaryExtensionTests {
        [Test]
        public void Using_AddOrChange_Insert_A_Value_Inside_A_Dictionary() {
            // arrange
            var dictionary = new Dictionary<string, object>();

            // act
            DictionaryExtension.AddOrChange(dictionary, "test", 123);

            // assert
            Assert.Multiple(() => {
                Assert.That(dictionary.First().Key, Is.EqualTo("test"));
                Assert.That(dictionary.First().Value, Is.EqualTo(123));
            });
        }

        [Test]
        public void Using_AddOrChange_Change_A_Value_Inside_A_Dictionary() {
            // arrange
            var dictionary = new Dictionary<string, object>();

            // 1st act
            DictionaryExtension.AddOrChange(dictionary, "test", 123);

            // 1st assert
            Assert.Multiple(() => {
                Assert.That(dictionary.First().Key, Is.EqualTo("test"));
                Assert.That(dictionary.First().Value, Is.EqualTo(123));
            });

            // 2nd act
            DictionaryExtension.AddOrChange(dictionary, "test", 789);

            // 2nd assert
            Assert.Multiple(() => {
                Assert.That(dictionary.First().Key, Is.EqualTo("test"));
                Assert.That(dictionary.First().Value, Is.EqualTo(789));
            });
        }
    }
}
