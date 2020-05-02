using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using Nameless.Core.Helpers;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;

namespace Nameless.Orm.NHibernate {
    /// <summary>
    /// Implementation for <see cref="string.Equals(string, string, StringComparison)"/>.
    /// </summary>
    public class StringEqualsHqlGeneratorForMethod : BaseHqlGeneratorForMethod {

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="StringEqualsHqlGeneratorForMethod"/>.
        /// </summary>
        public StringEqualsHqlGeneratorForMethod () {
            SupportedMethods = new[] {
                ExpressionHelper.GetMethodDefinition<string>(_ => _.Equals(null, StringComparison.CurrentCulture))
            };
        }

        #endregion

        #region Public Override Methods

        /// <inheritdoc />
        public override HqlTreeNode BuildHql (MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor) {
            var comparison = (StringComparison)Expression.Constant (arguments[1], typeof (StringComparison)).Value;
            if (comparison == StringComparison.CurrentCultureIgnoreCase ||
                comparison == StringComparison.InvariantCultureIgnoreCase ||
                comparison == StringComparison.OrdinalIgnoreCase) {
                return treeBuilder.Equality (
                    treeBuilder.MethodCall ("lower", new[] { visitor.Visit (targetObject).AsExpression () }),
                    treeBuilder.MethodCall ("lower", new[] { visitor.Visit (arguments[0]).AsExpression () }));
            }
            return treeBuilder.Equality (
                visitor.Visit (targetObject).AsExpression (),
                visitor.Visit (arguments[0]).AsExpression ());
        }

        #endregion
    }
}