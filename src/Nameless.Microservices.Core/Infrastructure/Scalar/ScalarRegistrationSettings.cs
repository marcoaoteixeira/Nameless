using Microsoft.AspNetCore.Http;
using Nameless.Registration;
using Scalar.AspNetCore;

namespace Nameless.Microservices.Infrastructure.Scalar;

public class ScalarRegistrationSettings : AssemblyScanAware<ScalarRegistrationSettings> {
    public Type? AuthorizationHttpClient { get; private set; }

    public Action<ScalarOptions, HttpContext>? ConfigureScalar { get; set; }

    public ScalarRegistrationSettings RegisterAuthorizationHttpClient<TAuthorizationHttpClient>()
        where TAuthorizationHttpClient : IAuthorizationHttpClient {
        return RegisterAuthorizationHttpClient(typeof(TAuthorizationHttpClient));
    }

    public ScalarRegistrationSettings RegisterAuthorizationHttpClient(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFrom(type, typeof(IAuthorizationHttpClient));

        AuthorizationHttpClient = type;

        return this;
    }
}