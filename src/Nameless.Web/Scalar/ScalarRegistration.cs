using Microsoft.AspNetCore.Http;
using Nameless.Registration;
using Scalar.AspNetCore;

namespace Nameless.Web.Scalar;

public class ScalarRegistration : AssemblyScanAware<ScalarRegistration> {
    /// <summary>
    ///     Gets the delegate to configure Scalar.
    /// </summary>
    public Action<ScalarOptions, HttpContext>? ConfigureScalar { get; set; }

    /// <summary>
    ///     Whether it should use the default JWT authentication/authorization.
    /// </summary>
    public bool UseDefaultHttpAuthentication { get; set; }
}