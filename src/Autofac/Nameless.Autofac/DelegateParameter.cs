using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac;
using Autofac.Core;

namespace Nameless.Autofac {

    public class DelegateParameter : Parameter {

        #region Private Read-Only Fields

        private readonly string _name;
        private readonly Func<object> _factory;

        #endregion

        #region Public Constructors

        public DelegateParameter(string name, Func<object> factory) {
            Prevent.NullOrWhiteSpaces(name, nameof(name));
            Prevent.Null(factory, nameof(factory));

            _name = name;
            _factory = factory;
        }

        #endregion

        #region Public Override Methods

        public override bool CanSupplyValue(ParameterInfo pi, IComponentContext context, [NotNullWhen(returnValue: true)] out Func<object>? valueProvider) {
            var propertyInfo = GetProperty(pi);
            if (propertyInfo == null || propertyInfo.Name != _name) {
                valueProvider = null;
                return false;
            }
            valueProvider = _factory;
            return true;
        }

        #endregion

        #region Private Static Methods

        private static PropertyInfo? GetProperty(ParameterInfo pi) {
            var methodInfo = pi.Member as MethodInfo;
            if (methodInfo == null) { return null; }

            if (methodInfo.IsSpecialName && (methodInfo.Name.StartsWith("set_", StringComparison.Ordinal) && methodInfo.DeclaringType != null))
                return methodInfo.DeclaringType.GetProperty(methodInfo.Name[4..]);
            return null;
        }

        #endregion
    }
}
