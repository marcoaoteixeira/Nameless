using Microsoft.AspNetCore.RateLimiting;
using Nameless.Registration;

namespace Nameless.Web.RateLimiter;

public class RateLimiterRegistration : AssemblyScanAware<RateLimiterRegistration> {
    private readonly Dictionary<string, Type> _rateLimiterPolicies = [];

    public IReadOnlyDictionary<string, Type> RateLimiterPolicies => UseAssemblyScan
        ? ExecuteAssemblyScan(typeof(IRateLimiterPolicy<>)).ToDictionary(
            keySelector: type => type.Name,
            elementSelector: type => type)
        : _rateLimiterPolicies;

    public RateLimiterRegistration WithRateLimiter<TRateLimiterPolicy, TPartitionKey>(string? name = null)
        where TRateLimiterPolicy : IRateLimiterPolicy<TPartitionKey> {
        return WithRateLimiter(typeof(TRateLimiterPolicy), name);
    }

    public RateLimiterRegistration WithRateLimiter(Type type, string? name = null) {
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IRateLimiterPolicy<>));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        name = string.IsNullOrWhiteSpace(name)
            ? RateLimiterPolicyAttribute.GetName(type)
            : name;

        _rateLimiterPolicies[name] = type;

        return this;
    }
}