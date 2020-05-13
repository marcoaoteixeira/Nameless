using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Nameless.Helpers {
    public static class ExpressionHelper {
        #region Public Static Methods

        public static MethodInfo GetMethodDefinition <TSource> (Expression<Action<TSource>> expression) {
            Prevent.ParameterNull (expression, nameof (expression));

            var methodInfo = ((MethodCallExpression)expression.Body).Method;
            return methodInfo.IsGenericMethod ? methodInfo.GetGenericMethodDefinition () : methodInfo;
        }

        #endregion
    }
}