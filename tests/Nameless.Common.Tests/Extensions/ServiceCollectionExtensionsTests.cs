using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nameless.Attributes;

namespace Nameless.Extensions;

public class ServiceCollectionExtensionsTests {
    // ─── ConfigureOptions ────────────────────────────────────────────────────

    [Fact]
    public void ConfigureOptions_WithNullConfiguration_RegistersWithDefaults() {
        // arrange
        var services = new ServiceCollection();

        // act
        services.ConfigureOptions<SampleOptions>(configuration: null);
        var provider = services.BuildServiceProvider();
        var opts = provider.GetRequiredService<IOptions<SampleOptions>>().Value;

        // assert
        Assert.NotNull(opts);
        Assert.Equal(0, opts.Value);
    }

    [Fact]
    public void ConfigureOptions_WithMatchingSection_BindsCorrectly() {
        // arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["SampleOptions:Value"] = "77"
            })
            .Build();

        var services = new ServiceCollection();

        // act
        services.ConfigureOptions<SampleOptions>(config);
        var provider = services.BuildServiceProvider();
        var opts = provider.GetRequiredService<IOptions<SampleOptions>>().Value;

        // assert
        Assert.Equal(77, opts.Value);
    }

    [Fact]
    public void ConfigureOptions_WithAttributeSection_UsesAttributeName() {
        // arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["CustomSection:Value"] = "55"
            })
            .Build();

        var services = new ServiceCollection();

        // act
        services.ConfigureOptions<AttributedOptions>(config);
        var provider = services.BuildServiceProvider();
        var opts = provider.GetRequiredService<IOptions<AttributedOptions>>().Value;

        // assert
        Assert.Equal(55, opts.Value);
    }

    [Fact]
    public void ConfigureOptions_WithDirectSection_UsesSection() {
        // arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["SampleOptions:Value"] = "33"
            })
            .Build();

        var section = config.GetSection("SampleOptions");
        var services = new ServiceCollection();

        // act
        services.ConfigureOptions<SampleOptions>(section);
        var provider = services.BuildServiceProvider();
        var opts = provider.GetRequiredService<IOptions<SampleOptions>>().Value;

        // assert
        Assert.Equal(33, opts.Value);
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    private sealed class SampleOptions {
        public int Value { get; set; }
    }

    [ConfigurationSectionName("CustomSection")]
    private sealed class AttributedOptions {
        public int Value { get; set; }
    }
}
