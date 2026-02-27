using Microsoft.AspNetCore.RateLimiting;
using Nameless.Registration;
using Nameless.Web.Attributes;

namespace Nameless.Web.RateLimiter;

public class RateLimiterRegistrationSettings : AssemblyScanAware<RateLimiterRegistrationSettings> {
    private readonly Dictionary<string, Type> _rateLimiterPolicies = [];

    public IReadOnlyDictionary<string, Type> RateLimiterPolicies => UseAssemblyScan
        ? GetImplementationsFor(typeof(IRateLimiterPolicy<>), includeGenericDefinition: false)
            .ToDictionary(
                keySelector: PolicyNameAttribute.GetName,
                elementSelector: type => type
            )
        : _rateLimiterPolicies;

    public RateLimiterRegistrationSettings RegisterRateLimiter<TRateLimiterPolicy, TPartitionKey>(string? name = null)
        where TRateLimiterPolicy : IRateLimiterPolicy<TPartitionKey> {
        return RegisterRateLimiter(typeof(TRateLimiterPolicy), name);
    }

    public RateLimiterRegistrationSettings RegisterRateLimiter(Type type, string? name = null) {
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IRateLimiterPolicy<>));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);
        Throws.When.HasNoParameterlessConstructor(type);

        name = string.IsNullOrWhiteSpace(name)
            ? PolicyNameAttribute.GetName(type)
            : name;

        _rateLimiterPolicies[name] = type;

        return this;
    }
}