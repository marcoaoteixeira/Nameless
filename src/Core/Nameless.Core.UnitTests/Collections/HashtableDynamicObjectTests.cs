using Nameless.Collections;

namespace Nameless.Core.UnitTests.Collections {

    public class HashtableDynamicObjectTests {

        [Test]
        public void HashtableDynamicObject_Create() {
            dynamic hash = new HashtableDynamicObject();

            hash.Property = "wewe";
        }
    }
}
