using System.Text;
using Xunit;

namespace Nameless.Serialization.Json.Test {
    public class JsonSerializerTest {
        [Fact]
        public void Serialize_WithSimpleData_ReturnsJson () {
            // arrange
            var serializer = new JsonSerializer ();
            var data = 1;

            // act
            var buffer = serializer.Serialize (data);
            var json = Encoding.UTF8.GetString (buffer);

            // assert 
            Assert.Equal ("1", json);
        }

        [Fact]
        public void Serialize_WithComplexData_ReturnsJson () {
            // arrange
            var serializer = new JsonSerializer ();
            var data = new {
                Name = "Test"
            };

            // act
            var buffer = serializer.Serialize (data);
            var json = Encoding.UTF8.GetString (buffer);

            // assert 
            Assert.Equal ("{\"Name\":\"Test\"}", json);
        }
    }
}