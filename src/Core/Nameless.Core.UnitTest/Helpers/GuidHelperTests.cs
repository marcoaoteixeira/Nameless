namespace Nameless.Helpers {
    public class GuidHelperTests {
        private readonly Guid GuidValue = Guid.Parse("126d40ca-d449-4f18-8fc3-43ec8646a36a");
        private const string StringValue = "ykBtEknUGE_Pw0Pshkajag";

        [Test]
        public void Encode_WhenPassGuidValue_ThenReturnStringEncoded() {
            // arrange

            // act
            var actual = GuidHelper.Encode(GuidValue);

            // assert
            Assert.Multiple(() => {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual, Is.EqualTo(StringValue));
            });
        }

        [Test]
        public void Decode_WhenPassStringValue_ThenReturnGuidDecoded() {
            // arrange

            // act
            var actual = GuidHelper.Decode(StringValue);

            // assert
            Assert.That(actual, Is.EqualTo(GuidValue));
        }
    }
}
