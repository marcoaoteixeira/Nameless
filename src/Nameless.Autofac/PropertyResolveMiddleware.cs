using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Resolving.Pipeline;

namespace Nameless.Autofac;

/// <summary>
///     Defines a resolve middleware to inject public properties.
/// </summary>
public sealed class PropertyResolveMiddleware : IResolveMiddleware {
    private readonly Func<MemberInfo, IComponentContext, object> _factory;
    private readonly Type _serviceType;

    /// <summary>
    ///     Initializes a new instance of <see cref="PropertyResolveMiddleware" />
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="factory">The factory function that will resolve the component.</param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="serviceType" /> or
    ///     <paramref name="factory" /> is <see langword="null"/>.
    /// </exception>
    public PropertyResolveMiddleware(Type serviceType, Func<MemberInfo, IComponentContext, object> factory) {
        _serviceType = Guard.Against.Null(serviceType);
        _factory = Guard.Against.Null(factory);
    }

    /// <inheritdoc />
    public PipelinePhase Phase => PipelinePhase.ParameterSelection;

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="context" /> or
    ///     <paramref name="next" /> is <see langword="null"/>.
    /// </exception>
    public void Execute(ResolveRequestContext context, Action<ResolveRequestContext> next) {
        Guard.Against.Null(context);
        Guard.Against.Null(next);

        context.ChangeParameters(context.Parameters.Union([
            new ResolvedParameter(
                predicate: (param, _) => param.ParameterType == _serviceType,
                valueAccessor: (param, ctx) => _factory(param.Member, ctx)
            )
        ]));

        next(context);

        if (context is not { NewInstanceActivated: true, Instance: not null }) {
            return;
        }

        var implementationType = context.Instance.GetType();
        var properties = GetProperties(implementationType, _serviceType);
        foreach (var property in properties) {
            property.SetValue(context.Instance,
                _factory(property, context),
                null);
        }
    }

    private static IEnumerable<PropertyInfo> GetProperties(Type implementationType, Type serviceType) {
        return implementationType
              .GetProperties(BindingFlags.Public | BindingFlags.Instance)
              .Where(prop => serviceType.IsAssignableFrom(prop.PropertyType) &&
                             prop.CanWrite &&
                             prop.GetIndexParameters().Length == 0);
    }
}