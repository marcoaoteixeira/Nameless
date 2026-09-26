using System.Reflection;
using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.RateLimiting;

namespace Nameless.Web.RateLimiter;


/// <summary>
///     Non-generic helper for registering
///     <see cref="IRateLimiterPolicy{TPartitionKey}"/> implementations when
///     the partition key type and policy type are only known at runtime (e.g.
///     read from a lookup dictionary or configuration), where a generic
///     method call isn't possible because the type arguments aren't known at
///     compile time.
/// </summary>
public static class RateLimiterOptionsExtensions {
    // Microsoft.AspNetCore.RateLimiting.RateLimiterOptionsExtensions has 4
    // overloads named "AddPolicy". This filter isolates the one shaped like:
    //      AddPolicy<TPartitionKey, TPolicy>(
    //          this RateLimiterOptions options,
    //          string policyName
    //      ) where TPolicy : IRateLimiterPolicy<TPartitionKey>
    // (the one that resolves TPolicy from DI), as opposed to the
    // Func<HttpContext, ...> overloads or the "object policyName" overloads.
    private static readonly MethodInfo AddPolicyHandler =
        typeof(RateLimiterOptions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(handler =>
                handler is { Name: nameof(RateLimiterOptions.AddPolicy), IsGenericMethodDefinition: true } &&
                handler.GetGenericArguments().Length == 2 &&
                handler.GetParameters().Length == 1 &&
                handler.GetParameters()[0].ParameterType == typeof(string)
            );

    /// <param name="self">
    ///     The current instance of <see cref="RateLimiterOptions"/> class.
    /// </param>
    extension(RateLimiterOptions self)
    {
        /// <summary>
        ///     Registers a rate limiter policy whose partition key type and
        ///     policy type are only known at runtime as <see cref="Type"/>
        ///     instances.
        /// </summary>
        /// <remarks>
        ///     Functionally equivalent to calling
        ///     <![CDATA[self.AddPolicy<TPartitionKey, TPolicy>(name)]]>, the
        ///     policy type is still resolved from DI at build time, exactly like
        ///     the generic overload.
        /// </remarks>
        /// <param name="partitionKeyType">
        ///     The partition key type (TPartitionKey).
        /// </param>
        /// <param name="rateLimiterPolicyType">
        ///     The policy type (TPolicy). Must implement
        ///     <see cref="IRateLimiterPolicy{TPartitionKey}"/> closed over
        ///     <paramref name="partitionKeyType"/>.
        /// </param>
        /// <param name="name">
        ///     The policy name used later in <c>RequireRateLimiting(name)</c>.
        /// </param>
        /// <returns>
        ///     The current instance of <see cref="RateLimiterOptions"/> to other
        ///     actions can be chained.
        /// </returns>
        public RateLimiterOptions AddPolicy(Type partitionKeyType, Type rateLimiterPolicyType, string name) {
            Throws.When.Null(self);
            Throws.When.Null(partitionKeyType);
            Throws.When.Null(rateLimiterPolicyType);
            Throws.When.Null(name);

            var service = typeof(IRateLimiterPolicy<>).MakeGenericType(partitionKeyType);
            if (!service.IsAssignableFrom(rateLimiterPolicyType)) {
                throw new ArgumentException(
                    message: $"'{rateLimiterPolicyType}' must implement '{service}'.",
                    paramName: nameof(rateLimiterPolicyType)
                );
            }

            var handler = AddPolicyHandler.MakeGenericMethod(partitionKeyType, rateLimiterPolicyType);

            try {
                // Instance method: 'self' is the invocation target, not a parameter.
                handler.Invoke(obj: self, parameters: [name]);
            }
            catch (TargetInvocationException ex) when (ex.InnerException is not null) {
                // Unwrap so callers see the real exception (and stack trace) instead of a
                // generic TargetInvocationException from the reflection call.
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            return self;
        }
    }
}
