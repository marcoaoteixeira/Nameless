using Moq.Internals;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Common.Testing.Tools;

[UnitTest]
public class QuickTests
{
    public interface IPrintService
    {
        void Print(string value);
    }

    [Fact]
    public void Quick_Can_Generate_Mock()
    {
        // act
        var actual = Quick.Mock<IPrintService>();

        // assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(actual);
            Assert.IsType<InterfaceProxy>(actual, exactMatch: false);
        });
    }
}
