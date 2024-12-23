using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Nameless;

/// <summary>
/// <see cref="Expression"/> extension methods.
/// </summary>
public static class ExpressionExtension {
    private static readonly object Empty = new();

    /// <summary>
    /// Retrieves the expression path.
    /// See <a href="https://github.com/dotnet/aspnetcore/blob/release/8.0/src/Mvc/Mvc.ViewFeatures/src/ExpressionHelper.cs">ExpressionHelper source code</a>
    /// </summary>
    /// <param name="self">The lambda expression.</param>
    /// <returns>The path of the expression.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static string GetExpressionPath(this LambdaExpression self) {
        Prevent.Argument.Null(self);

        // Determine size of string needed (length) and number of segments it contains
        // (segmentCount). Put another way, segmentCount tracks the number of times the
        // loop below should iterate. This avoids adding ".model" and / or an extra
        // leading "." and then removing them after the loop. Other information collected
        // in this first loop helps with length and segmentCount adjustments. doNotCache
        // is somewhat separate: If true, expression strings are not cached for the
        // expression.
        //
        // After the corrections below the first loop, length is usually exactly the size
        // of the returned string. However, when containsIndexers is true, the calculation
        // is approximate because either evaluating indexer expressions multiple times or
        // saving indexer strings can get expensive. Optimizing for the common case of a
        // collection (not a dictionary) with less than 100 elements. If that assumption
        // proves to be incorrect, the StringBuilder will be enlarged but hopefully just
        // once.
        
        // ReSharper disable InconsistentNaming
        const int MaxIndexerLength = 4; // "[99]"
        const string ImplReservedSign = "__";
        // ReSharper restore InconsistentNaming
        
        var length = 0;
        var segmentCount = 0;
        var trailingMemberExpressions = 0;
        var part = self.Body;
        string? name;

        while (part is not null) {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (part.NodeType) {
                case ExpressionType.Call:
                    // Will exit loop if at Method().Property or [i,j].Property. In that case
                    // (like [i].Property), don't cache and don't remove ".Model"
                    // (if that's .Property).
                    var methodExpression = (MethodCallExpression)part;
                    if (IsSingleArgumentIndexer(methodExpression)) {
                        length += MaxIndexerLength;
                        part = methodExpression.Object;
                        segmentCount++;
                        trailingMemberExpressions = 0;
                    } else { part = null; /* Set Unsupported */ }

                    break;

                case ExpressionType.ArrayIndex:
                    var binaryExpression = (BinaryExpression)part;
                    length += MaxIndexerLength;
                    part = binaryExpression.Left;
                    segmentCount++;
                    trailingMemberExpressions = 0;

                    break;

                case ExpressionType.MemberAccess:
                    var memberExpressionPart = (MemberExpression)part;
                    name = memberExpressionPart.Member.Name;

                    // If identifier contains "__", it is "reserved for use by the implementation"
                    // and likely compiler- or Razor-generated e.g. the name of a field in a
                    // delegate's generated class.
                    if (name.Contains(ImplReservedSign))
                    {
                        // Exit loop.
                        part = null;
                        break;
                    }

                    length += name.Length + 1;
                    part = memberExpressionPart.Expression;
                    segmentCount++;
                    trailingMemberExpressions++;

                    break;

                case ExpressionType.Parameter:
                    var parameterExpressionPart = (ParameterExpression)part;

                    name = parameterExpressionPart.Name;

                    if (name is not null) {
                        length += name.Length + 1;
                    }

                    // We are at the start of the expression, let's exist.
                    part = null;
                    segmentCount++;
                    trailingMemberExpressions++;

                    break;

                default:
                    // Unsupported.
                    part = null;

                    break;
            }
        }

        // Trim the leading "." if present. The loop below special-cases the last
        // property to avoid this addition.
        if (trailingMemberExpressions > 0) {
            length--;
        }

        if (segmentCount == 0) {
            return string.Empty;
        }

        var builder = new StringBuilder(length);
        part = self.Body;
        while (part is not null && segmentCount > 0) {
            segmentCount--;
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (part.NodeType) {
                case ExpressionType.Call:
                    var methodExpression = (MethodCallExpression)part;

                    InsertIndexerInvocationText(builder, methodExpression.Arguments.Single(), self);

                    part = methodExpression.Object;
                    break;

                case ExpressionType.ArrayIndex:
                    var binaryExpression = (BinaryExpression)part;

                    InsertIndexerInvocationText(builder, binaryExpression.Right, self);

                    part = binaryExpression.Left;
                    break;

                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)part;

                    name = memberExpression.Member.Name;

                    builder.Insert(0, name);
                    if (segmentCount > 0) {
                        // One or more parts to the left of this part are coming.
                        builder.Insert(0, Separators.DOT);
                    }

                    part = memberExpression.Expression;
                    break;

                case ExpressionType.Parameter:
                    var parameterExpression = (ParameterExpression)part;

                    name = parameterExpression.Name;

                    builder.Insert(0, name);
                    if (segmentCount > 0) {
                        // One or more parts to the left of this part are coming.
                        builder.Insert(0, Separators.DOT);
                    }

                    part = null;
                    break;
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Creates an AND condition with another expression.
    /// </summary>
    /// <typeparam name="T">Type of the expression.</typeparam>
    /// <param name="self">The left expression.</param>
    /// <param name="expression">The right expression.</param>
    /// <returns>An expression composition.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> or
    /// <paramref name="expression"/> is <c>null</c>.
    /// </exception>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> self, Expression<Func<T, bool>> expression) {
        Prevent.Argument.Null(self);
        Prevent.Argument.Null(expression);

        var param = Expression.Parameter(typeof(T), Separators.UNDERSCORE);
        var body = Expression.And(
            left: Expression.Invoke(self, param),
            right: Expression.Invoke(expression, param)
        );
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    /// <summary>
    /// Creates an OR condition with another expression.
    /// </summary>
    /// <typeparam name="T">Type of the expression.</typeparam>
    /// <param name="self">The left expression.</param>
    /// <param name="expression">The right expression.</param>
    /// <returns>An expression composition.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/>
    /// or <paramref name="expression"/> is <c>null</c>.
    /// </exception>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> self, Expression<Func<T, bool>> expression) {
        Prevent.Argument.Null(self);
        Prevent.Argument.Null(expression);

        var param = Expression.Parameter(typeof(T), Separators.UNDERSCORE);
        var body = Expression.Or(
            left: Expression.Invoke(self, param),
            right: Expression.Invoke(expression, param)
        );
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    private static void InsertIndexerInvocationText(StringBuilder builder, Expression indexExpression, LambdaExpression parentExpression) {
        var convert = Expression.Convert(indexExpression, typeof(object));
        var parameter = Expression.Parameter(typeof(object), null);
        var lambda = Expression.Lambda<Func<object, object>>(convert, parameter);
        Func<object, object> func;

        try { func = lambda.Compile(); }
        catch (InvalidOperationException ex) {
            throw new InvalidOperationException(
                message: $"Invalid indexer expression. {indexExpression} : {parentExpression.Parameters[0].Name}",
                innerException: ex
            );
        }

        builder.Insert(0, ']');
        builder.Insert(0, Convert.ToString(func(Empty), CultureInfo.InvariantCulture));
        builder.Insert(0, '[');
    }

    private static bool IsSingleArgumentIndexer(Expression expression) {
        if (expression is not MethodCallExpression methodExpression || methodExpression.Arguments.Count != 1) {
            return false;
        }

        // Check whether GetDefaultMembers() (if present in CoreCLR) would return a
        // member of this type. Compiler names the indexer property, if any, in a
        // generated [DefaultMember] attribute for the containing type.
        var declaringType = methodExpression.Method.DeclaringType!;
        var defaultMember = declaringType.GetCustomAttribute<DefaultMemberAttribute>(inherit: true);
        if (defaultMember is null) {
            return false;
        }

        // Find default property (the indexer) and confirm its getter is the method in
        // this expression.
        var runtimeProperties = declaringType.GetRuntimeProperties();
        return runtimeProperties.Any(property => string.Equals(defaultMember.MemberName, property.Name, StringComparison.Ordinal) &&
                                                 property.GetMethod == methodExpression.Method);
    }
}