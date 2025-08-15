using System.Diagnostics.CodeAnalysis;

namespace Nameless.Web.Identity.Objects;

public abstract record Response {
    public string? Error { get; init; }

    [MemberNotNullWhen(returnValue: false, nameof(Error))]
    public virtual bool Succeeded => string.IsNullOrWhiteSpace(Error);
}