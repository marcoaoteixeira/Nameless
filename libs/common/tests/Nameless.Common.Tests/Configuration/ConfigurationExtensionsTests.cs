using Microsoft.Extensions.Configuration;

namespace Nameless.Configuration;

[UnitTest]
public class ConfigurationExtensionsTests {
    // ─── GetSectionFor ──────────────────────────────────────────────────────────
    [ConfigurationSectionName("Foo")]
    private sealed class FooOptions {
        public string? Value { get; set; }
    }

    private static IConfigurationRoot Build(params (string Key, string Value)[] values) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(values.Select(v => new KeyValuePair<string, string?>(v.Key, v.Value)))
            .Build();

    [Fact]
    public void Root_ResolvesSectionNameFromAttribute() {
        var config = Build(("Foo:Value", "x"));

        var result = config.GetSectionFor<FooOptions>();

        Assert.Equal("Foo", result.Path);
        Assert.Equal("x", result["Value"]);
    }

    [Fact]
    public void Root_ExplicitSectionName_OverridesAttribute() {
        var config = Build(("Foo:Value", "foo"), ("Bar:Value", "bar"));

        var result = config.GetSectionFor<FooOptions>("Bar");

        Assert.Equal("Bar", result.Path);
        Assert.Equal("bar", result["Value"]);
    }

    [Fact]
    public void CurrentSectionMatchesKey_ReturnsSameInstance() {
        var config = Build(("Foo:Value", "x"));
        var foo = config.GetSection("Foo");

        var result = foo.GetSectionFor<FooOptions>();

        Assert.Same(foo, result);
    }

    [Theory]
    [InlineData("FOO")]
    [InlineData("foo")]
    [InlineData("fOo")]
    public void CurrentSectionMatchesKey_IsCaseInsensitive(string requestedName) {
        var config = Build(("Foo:Value", "x"));
        var foo = config.GetSection("Foo");

        var result = foo.GetSectionFor<FooOptions>(requestedName);

        Assert.Same(foo, result);
    }

    [Fact]
    public void CurrentSectionHasDifferentKey_ReturnsChildSection() {
        var config = Build(("Parent:Foo:Value", "x"));
        var parent = config.GetSection("Parent");

        var result = parent.GetSectionFor<FooOptions>();

        Assert.NotSame(parent, result);
        Assert.Equal("Parent:Foo", result.Path);
        Assert.Equal("x", result["Value"]);
    }

    [Fact]
    public void NestedSectionMatchesKey_ReturnsSelf_EvenWhenNotAtRoot() {
        var config = Build(("App:Foo:Value", "x"));
        var foo = config.GetSection("App:Foo");

        var result = foo.GetSectionFor<FooOptions>();

        Assert.Same(foo, result);
        Assert.Equal("App:Foo", result.Path);
    }

    [Fact]
    public void Caveat_SectionWithSameNamedChild_ReturnsParent() {
        var config = Build(("Foo:Value", "outer"), ("Foo:Foo:Value", "inner"));
        var outer = config.GetSection("Foo");

        var result = outer.GetSectionFor<FooOptions>();

        // Key is compared, not Path: the parent wins over the Foo:Foo child.
        Assert.Same(outer, result);
        Assert.Equal("Foo", result.Path);
        Assert.Equal("outer", result["Value"]);
    }

    [Fact]
    public void Caveat_SameNamedChild_IsStillReachableThroughPlainGetSection() {
        var config = Build(("Foo:Foo:Value", "inner"));
        var outer = config.GetSection("Foo");

        var inner = outer.GetSection("Foo");

        Assert.Equal("Foo:Foo", inner.Path);
        Assert.Equal("inner", inner["Value"]);
    }

    [Fact]
    public void CompositeName_FromRoot_ResolvesByPath() {
        var config = Build(("Features:Foo:Value", "x"));

        var result = config.GetSectionFor<FooOptions>("Features:Foo");

        Assert.Equal("Features:Foo", result.Path);
        Assert.Equal("x", result["Value"]);
    }

    [Fact]
    public void CompositeName_NeverMatchesKey_FallsThroughToGetSection() {
        var config = Build(("Features:Foo:Value", "x"));
        var foo = config.GetSection("Features:Foo");

        var result = foo.GetSectionFor<FooOptions>("Features:Foo");

        Assert.NotSame(foo, result);
        Assert.Equal("Features:Foo:Features:Foo", result.Path);
        Assert.False(result.Exists());
    }

    [Fact]
    public void MissingSection_ReturnsNonNullSectionThatDoesNotExist() {
        var config = Build(("Other:Value", "x"));

        var result = config.GetSectionFor<FooOptions>();

        Assert.NotNull(result);
        Assert.Equal("Foo", result.Path);
        Assert.False(result.Exists());
    }

    [Fact]
    public void MissingSection_FromNonMatchingSection_ReturnsNonExistingChild() {
        var config = Build(("Parent:Other", "x"));
        var parent = config.GetSection("Parent");

        var result = parent.GetSectionFor<FooOptions>();

        Assert.NotNull(result);
        Assert.False(result.Exists());
    }


    // ─── GetOrCreate ──────────────────────────────────────────────────────────

    [Fact]
    public void GetOrCreate_WithMatchingSection_ReturnsBindedOptions() {
        // arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["MySection:Value"] = "42",
                ["MySection:Label"] = "Hello"
            })
            .Build();

        // act
        var opts = config.GetSectionFor<MyOptions>("MySection").GetOrCreate<MyOptions>();

        // assert
        Assert.Multiple(
            () => Assert.Equal(42, opts.Value),
            () => Assert.Equal("Hello", opts.Label)
        );
    }

    [Fact]
    public void GetOrCreate_WithMissingSection_ReturnsDefaultInstance() {
        // arrange
        var config = new ConfigurationBuilder().AddInMemoryCollection([])
                                               .Build();

        // act
        var opts = config.GetSectionFor<MyOptions>("MissingSection").GetOrCreate<MyOptions>();

        // assert
        Assert.Multiple(
            () => Assert.NotNull(opts),
            () => Assert.Equal(0, opts.Value)
        );
    }

    [Fact]
    public void GetOrCreate_WithConfigSectionNameAttribute_UsesAttributeName() {
        // arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["AttributeSection:Value"] = "99"
            })
            .Build();

        // act
        var opts = config.GetSectionFor<AttributeOptions>().GetOrCreate<AttributeOptions>();

        // assert
        Assert.Equal(99, opts.Value);
    }

    // ─── GetAll ──────────────────────────────────────────────────

    [Fact]
    public void GetAll_WithMultipleChildren_ReturnsDictionary() {
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
        var opts = config.GetAll<WorkerOptions>(sectionName: "Workers");

        // assert
        Assert.Multiple(
            () => Assert.Equal(2, opts.Count),
            () => Assert.True(opts["WorkerA"].IsEnabled),
            () => Assert.False(opts["WorkerB"].IsEnabled)
        );
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

    [ConfigurationSectionName("Fake")]
    private sealed class FakeSection {
        public int Value { get; set; }
    }
}
