using System.Linq;
using Nameless.Collections.Generic;

namespace Nameless.Core.UnitTests.Collections.Generic {

    public class AsyncEnumerableTests {

        [Test]
        public void Can_Create_AsyncEnumerable() {
            // arrange
            var items = new[] { 1, 2, 3, 4, 5 }.AsEnumerable();

            // act
            var enumerable = new AsyncEnumerable<int>(items);

            // assert

        }
    }
}
