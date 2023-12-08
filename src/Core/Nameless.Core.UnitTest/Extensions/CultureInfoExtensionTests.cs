using System.Globalization;

namespace Nameless.Core.UnitTest.Extensions {
    public class CultureInfoExtensionTests {
        [Test]
        public void Get_All_Children_Cultures_From_A_Specific_Culture() {
            var culture = new CultureInfo("en-EN");

            // en-EN culture has en as child culture, so you'll get 2 cultures
            var cultures = CultureInfoExtension.GetParents(culture).ToArray();

            Assert.That(cultures, Has.Length.EqualTo(2));
        }
    }
}
