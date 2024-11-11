using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless;

public static class LoggerExtension {
    public static ILogger<TCategory> OnCondition<TCategory>(this ILogger<TCategory> self, bool condition)
        => condition ? Prevent.Argument.Null(self) : NullLogger<TCategory>.Instance;

    public static ILogger<TCategory> OnCondition<TCategory>(this ILogger<TCategory> self, Func<bool> condition)
        => OnCondition(self, condition());
}

