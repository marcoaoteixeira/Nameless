using System.Text;

namespace Nameless.Core.UnitTests {
    public class ByteArrayExtensionTests {

        [Test]
        public void ByteArrayExtension_Convert_Byte_Array_To_Hex_String() {
            // arrange
            var byteArray = Encoding.UTF8.GetBytes("this is a test");
            var sb = new StringBuilder();
            foreach (var item in byteArray) {
                sb.AppendFormat("{0:x2}", item);
            }
            var hex = sb.ToString();

            // act
            var hexString = ByteArrayExtension.ToHexString(byteArray);

            // assert
            Assert.That(hexString, Is.EqualTo(hex));
        }
    }
}
