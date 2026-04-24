using Microsoft.Extensions.Configuration;
using Nameless.Attributes;

namespace Nameless.Extensions;

public class ConfigurationExtensionsTests {
    // ─── GetOptions ──────────────────────────────────────────────────────────

    [Fact]
    public void GetOptions_WithMatchingSection_ReturnsBindedOptions() {
        // arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["MySection:Value"] = "42",
                ["MySection:Label"] = "Hello"
            })
            .Build();

        // act
        var opts = config.GetOptions<MyOptions>(sectionName: "MySection");

        // assert
        Assert.Multiple(() => {
            Assert.Equal(42, opts.Value);
            Assert.Equal("Hello", opts.Label);
        });
    }

    [Fact]
    public void GetOptions_WithMissingSection_ReturnsDefaultInstance() {
        // arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // act
        var opts = config.GetOptions<MyOptions>(sectionName: "MissingSection");

        // assert
        Assert.NotNull(opts);
        Assert.Equal(0, opts.Value);
    }

    [Fact]
    public void GetOptions_WithConfigSectionNameAttribute_UsesAttributeName() {
        // arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["AttributeSection:Value"] = "99"
            })
            .Build();

        // act
        var opts = config.GetOptions<AttributeOptions>();

        // assert
        Assert.Equal(99, opts.Value);
    }

    // ─── GetMultipleOptions ──────────────────────────────────────────────────

    [Fact]
    public void GetMultipleOptions_WithMultipleChildren_ReturnsDictionary() {
        // arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["Workers:WorkerA:IsEnabled"] = "true",
                ["Workers:WorkerA:Interval"] = "00:00:01",
                ["Workers:WorkerB:IsEnabled"] = "false",
                ["Workers:WorkerB:Interval"] = "00:00:02"
            })
            .Build();

        // act
        var opts = config.GetMultipleOptions<WorkerOptions>(sectionName: "Workers");

        // assert
        Assert.Multiple(() => {
            Assert.Equal(2, opts.Count);
            Assert.True(opts["WorkerA"].IsEnabled);
            Assert.False(opts["WorkerB"].IsEnabled);
        });
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    private sealed class MyOptions {
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;
    }

    [ConfigurationSectionName("AttributeSection")]
    private sealed class AttributeOptions {
        public int Value { get; set; }
    }

    private sealed class WorkerOptions {
        public bool IsEnabled { get; set; }
        public TimeSpan Interval { get; set; }
    }
}
