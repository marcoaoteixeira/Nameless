namespace Nameless {
    public class CharExtensionTests {
        [TestCase(true, new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' })]
        [TestCase(true, new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' })]
        [TestCase(false, new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '!', '@', '#', '$', '%', '¨', '&', '*', '(', ')', '-', '_', '=', '+' })]
        public void IsLetter_Returns_True_When_Letter(bool isLetter, char[] characteres) {
            // arrange
            var bools = new bool[characteres.Length];
            // act
            for (var idx = 0; idx < bools.Length; idx++) {
                bools[idx] = CharExtension.IsLetter(characteres[idx]);
            }

            // assert
            Assert.That(bools, Is.All.EqualTo(isLetter));
        }
    }
}
