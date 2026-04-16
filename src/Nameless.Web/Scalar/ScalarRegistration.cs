using Microsoft.AspNetCore.Http;
using Nameless.Registration;
using Scalar.AspNetCore;

namespace Nameless.Web.Scalar;

public class ScalarRegistration : AssemblyScanAware<ScalarRegistration> {
    public Action<ScalarOptions, HttpContext>? ConfigureScalar { get; set; }
}