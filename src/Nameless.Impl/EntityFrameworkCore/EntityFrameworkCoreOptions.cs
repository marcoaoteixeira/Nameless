using Nameless.Attributes;

namespace Nameless.EntityFrameworkCore;

[ConfigurationSectionName("EntityFrameworkCore")]
public record EntityFrameworkCoreOptions {
    public string ConnectionStringName { get; init; } = "default";
}