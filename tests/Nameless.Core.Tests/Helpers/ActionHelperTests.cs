using Nameless.Helpers;

namespace Nameless;

public class ActionHelperTests {
    // --- FromDelegate ---

    [Fact]
    public void FromDelegate_WithAction_CreatesAndConfiguresInstance() {
        // act
        var result = ActionHelper.FromDelegate<SampleConfig>(cfg => {
            cfg.Name = "configured";
            cfg.Value = 99;
        });

        // assert
        Assert.Multiple(() => {
            Assert.Equal("configured", result.Name);
            Assert.Equal(99, result.Value);
        });
    }

    [Fact]
    public void FromDelegate_WithNullAction_ReturnsDefaultInstance() {
        // act
        var result = ActionHelper.FromDelegate<SampleConfig>(null);

        // assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.Name);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public void FromDelegate_WithAction_ReturnsSameInstancePassedToAction() {
        // arrange
        SampleConfig? capturedRef = null;

        // act
        var result = ActionHelper.FromDelegate<SampleConfig>(cfg => capturedRef = cfg);

        // assert
        Assert.Same(capturedRef, result);
    }

    // --- test doubles ---

    private sealed class SampleConfig {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
