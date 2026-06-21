using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Nameless.Diagnostics.CodeAnalysis;
using Nameless.Registration;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="IResponseCookies"/> that does not store any cookies.
/// </summary>
[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.TrivialCode)]
[IgnoreAssemblyScan]
public sealed class NullResponseCookies : IResponseCookies {
    public static IResponseCookies Instance { get; } = new NullResponseCookies();

    static NullResponseCookies() { }

    private NullResponseCookies() { }

    /// <inheritdoc />
    public void Append(string key, string value) { }

    /// <inheritdoc />
    public void Append(string key, string value, CookieOptions options) { }

    /// <inheritdoc />
    public void Delete(string key) { }

    /// <inheritdoc />
    public void Delete(string key, CookieOptions options) { }
}