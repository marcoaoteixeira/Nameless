using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Nameless.Lucene.Repository.Extensions;

namespace Nameless.Lucene.Repository.Mappings;

public class EntityDescriptor<TDocument> : IEntityDescriptor<TDocument>
    where TDocument : class {
    private readonly Dictionary<string, PropertyDescriptor<TDocument>> _properties = [];

    public IReadOnlyCollection<PropertyDescriptor> Properties => _properties.Values;

    public IEntityDescriptor<TDocument> SetID<TProperty>(Expression<Func<TDocument, TProperty>> expression) {
        if (this.HasID) {
            throw new InvalidOperationException("An ID property was already defined.");
        }

        IncludeProperty(expression, isID: true, PropertyOptions.Store);

        return this;
    }

    public IEntityDescriptor<TDocument> SetProperty<TProperty>(Expression<Func<TDocument, TProperty>> expression, PropertyOptions options) {
        IncludeProperty(expression, isID: false, options);

        return this;
    }

    private void IncludeProperty<TProperty>(Expression<Func<TDocument, TProperty>> expression, bool isID, PropertyOptions options) {
        ValidateExpression(expression, out var name);

        _properties[name] = new PropertyDescriptor<TDocument> {
            Name = name,
            Type = typeof(TProperty),
            Options = options,
            IsID = isID,
            Getter = CreateGetter(expression),
            Setter = CreateSetter(expression),
        };
    }

    private static void ValidateExpression<TProperty>(Expression<Func<TDocument, TProperty>> expression, [NotNull] out string? name) {
        name = null;

        if (expression.Body is not MemberExpression member) {
            throw new ArgumentException("Expression must point to a property.", nameof(expression));
        }

        if (member.Member is not PropertyInfo propertyInfo) {
            throw new ArgumentException($"'{member.Member.Name}' must be a property.");
        }

        if (propertyInfo.GetMethod is null || !propertyInfo.GetMethod.IsPublic) {
            throw new ArgumentException($"'{member.Member.Name}' must has a public getter.");
        }

        if (propertyInfo.SetMethod is null || !propertyInfo.SetMethod.IsPublic) {
            throw new ArgumentException($"'{member.Member.Name}' must has a public setter.");
        }

        name = member.Member.Name;
    }

    private static Func<TDocument, object?> CreateGetter<TProperty>(Expression<Func<TDocument, TProperty>> expression) {
        return instance => expression.Compile().Invoke(instance);
    }

    private static Action<TDocument, object?> CreateSetter<TProperty>(Expression<Func<TDocument, TProperty>> expression) {
        var memberExpression = (MemberExpression)expression.Body;
        var instanceParameter = Expression.Parameter(typeof(TDocument), "instance");
        var valueParameter = Expression.Parameter(typeof(object), "value");

        // Cast the "value" object to the correct type.
        var castValue = Expression.Convert(valueParameter, typeof(TProperty));

        // Assign the value to the property.
        var assign = Expression.Assign(
            Expression.MakeMemberAccess(instanceParameter, memberExpression.Member),
            castValue
        );

        return Expression.Lambda<Action<TDocument, object?>>(
            assign,
            instanceParameter,
            valueParameter
        ).Compile();
    }
}