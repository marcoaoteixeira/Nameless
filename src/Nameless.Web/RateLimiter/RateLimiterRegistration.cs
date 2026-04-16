using Microsoft.AspNetCore.RateLimiting;
using Nameless.Registration;

namespace Nameless.Web.RateLimiter;

public class RateLimiterRegistration : AssemblyScanAware<RateLimiterRegistration> {
    private readonly Dictionary<string, Type> _rateLimiterPolicies = [];

    public IReadOnlyDictionary<string, Type> RateLimiterPolicies => _rateLimiterPolicies;

    public RateLimiterRegistration RegisterRateLimiter<TRateLimiterPolicy, TPartitionKey>(string? name = null)
        where TRateLimiterPolicy : IRateLimiterPolicy<TPartitionKey> {
        return RegisterRateLimiter(typeof(TRateLimiterPolicy), name);
    }

    public RateLimiterRegistration RegisterRateLimiter(Type type, string? name = null) {
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IRateLimiterPolicy<>));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);
        Throws.When.HasNoParameterlessConstructor(type);

        name = string.IsNullOrWhiteSpace(name)
            ? RateLimiterPolicyAttribute.GetName(type)
            : name;

        _rateLimiterPolicies[name] = type;

        return this;
    }
}