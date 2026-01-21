using Microsoft.Extensions.Options;

namespace Nameless.Testing.Tools.Helpers;

/// <summary>
///     Simple class to help with <see cref="IOptions{TOptions}"/>
/// </summary>
public static class OptionsHelper {
    public static IOptions<TOptions> Create<TOptions>(Action<TOptions>? configure = null)
        where TOptions : class, new() {
        var innerConfigure = configure ?? (_ => { });
        var options = new TOptions();

        innerConfigure(options);

        return Options.Create(options);
    }
}