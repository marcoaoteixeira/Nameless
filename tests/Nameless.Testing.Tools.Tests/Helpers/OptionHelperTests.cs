using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;

namespace Nameless.Common.Testing.Tools.Helpers;

[UnitTest]
public class OptionHelperTests
{
    public class TestOptions
    {
        public string? ValueA { get; set; }
        public string? ValueB { get; set; }
        public string? ValueC { get; set; }
    }

    [Fact]
    public void WhenCreating_ThenReturnsOptionInstance()
    {
        // arrange
        
        // act
        var opts = OptionsHelper.Create<TestOptions>();

        // assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(opts);
            Assert.NotNull(opts.Value);
            Assert.Null(opts.Value.ValueA);
            Assert.Null(opts.Value.ValueB);
            Assert.Null(opts.Value.ValueC);
        });
    }

    [Fact]
    public void WhenCreating_WithConfiguring_ThenReturnsOptionInstanceWithConfiguredValues()
    {
        // arrange
        const string Message = "It works!";

        // act
        var opts = OptionsHelper.Create<TestOptions>(config => config.ValueB = Message);

        // assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(opts);
            Assert.NotNull(opts.Value);
            Assert.Null(opts.Value.ValueA);
            Assert.Equal(Message, opts.Value.ValueB);
            Assert.Null(opts.Value.ValueC);
        });
    }
}