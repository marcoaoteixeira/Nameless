using Nameless.Collections.Generic;

namespace Nameless.Core.UnitTests.Collections.Generic {

    public class AsyncEnumerableTests {
        private static readonly int[] sourceArray = [1, 2, 3, 4, 5];

        [Test]
        public void Can_Create_AsyncEnumerable() {
            // arrange
            var items = sourceArray.AsEnumerable();

            // act
            var enumerable = new AsyncEnumerable<int>(items);

            // assert
            Assert.That(enumerable, Is.Not.Null);
        }

        [Test]
        public void GetAsyncEnumerator_From_AsyncEnumerable() {
            // arrange
            var items = sourceArray.AsEnumerable();

            // act
            var enumerator = new AsyncEnumerable<int>(items).GetAsyncEnumerator(CancellationToken.None);

            // assert
            Assert.That(enumerator, Is.Not.Null);
        }

        [Test]
        public async Task Consume_An_AsyncEnumerable() {
            // arrange
            static async IAsyncEnumerable<int> FetchItems() {
                for (var idx = 0; idx < 10; idx++) {
                    // simulated a delay
                    await Task.Delay(25);

                    yield return idx + 1;
                }
            };

            var result = new List<int>();
            // act
            await foreach (var item in FetchItems()) {
                result.Add(item);
            }

            // assert
            Assert.That(result, Has.Count.EqualTo(10));
        }
    }
}
