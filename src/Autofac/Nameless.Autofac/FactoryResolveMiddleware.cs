using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Resolving.Pipeline;

namespace Nameless.Autofac {

    public sealed class FactoryResolveMiddleware : IResolveMiddleware {

        #region Private Read-Only Fields

        private readonly Type _serviceType;
        private readonly Func<MemberInfo, IComponentContext, object> _factory;

        #endregion

        #region Public Constructors

        public FactoryResolveMiddleware(Type serviceType, Func<MemberInfo, IComponentContext, object> factory) {
            Prevent.Null(serviceType, nameof(serviceType));
            Prevent.Null(factory, nameof(factory));

            _serviceType = serviceType;
            _factory = factory;
        }

        #endregion

        #region Private Static Methods

        private static PropertyInfo[] GetProperties(Type implementationType, Type serviceType) => implementationType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(_ =>
                _.PropertyType.IsAssignableTo(serviceType) &&
                _.CanWrite &&
                _.GetIndexParameters().Any() == false
            ).ToArray();

        #endregion

        #region IResolveMiddleware Members

        public PipelinePhase Phase => PipelinePhase.ParameterSelection;

        public void Execute(ResolveRequestContext context, Action<ResolveRequestContext> next) {
            context.ChangeParameters(context.Parameters.Union(
                new[] {
                    new ResolvedParameter(
                        predicate: (param, ctx) => param.ParameterType == _serviceType,
                        valueAccessor: (param, ctx) => _factory(param.Member, ctx)
                    )
                }
            ));

            next(context);

            if (context.NewInstanceActivated) {
                var implementationType = context.Instance!.GetType();
                var properties = GetProperties(implementationType, _serviceType);
                foreach (var property in properties) {
                    property.SetValue(
                        obj: context.Instance,
                        value: _factory(property, context),
                        index: default
                    );
                }
            }
        }

        #endregion
    }
}