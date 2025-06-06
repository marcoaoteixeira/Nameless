using System.Globalization;

namespace Nameless;

public class CultureInfoExtensionsTests {
    [Fact]
    public void Get_All_Children_Cultures_From_A_Specific_Culture() {
        var culture = new CultureInfo("en-EN");

        // en-EN culture has en as child culture, so you'll get 2 cultures
        var cultures = culture.GetParents().ToArray();

        Assert.Equal(expected: 2, cultures.Length);
    }
}