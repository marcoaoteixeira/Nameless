using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Nameless.Lucene.Mapping;

public abstract class EntityMapping<TEntity> : IEntityMapping
    where TEntity : class {
    private readonly Dictionary<string, PropertyDescriptor<TEntity>> _properties = [];

    public Type Type => typeof(TEntity);

    public IReadOnlyCollection<PropertyDescriptor> Entries => _properties.Values;

    public void ID<TProperty>(Expression<Func<TEntity, TProperty>> expression) {
        ValidateExpression(expression, out _);

        _properties[Constants.DOCUMENT_ID_PROP] = new PropertyDescriptor<TEntity> {
            Name = Constants.DOCUMENT_ID_PROP,
            Type = typeof(TProperty),
            Getter = CreateGetter(expression),
            Setter = CreateSetter(expression),
            Options = PropertyOptions.Store
        };
    }

    public void Property<TProperty>(Expression<Func<TEntity, TProperty>> expression, PropertyOptions options) {
        ValidateExpression(expression, out var name);

        _properties[name] = new PropertyDescriptor<TEntity> {
            Name = name,
            Type = typeof(TProperty),
            Getter = CreateGetter(expression),
            Setter = CreateSetter(expression),
            Options = options
        };
    }

    private static void ValidateExpression<TProperty>(Expression<Func<TEntity, TProperty>> expression, [NotNull]out string? name) {
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

    private static Func<TEntity, object?> CreateGetter<TProperty>(Expression<Func<TEntity, TProperty>> expression) {
        return instance => expression.Compile().Invoke(instance);
    }

    private static Action<TEntity, object?> CreateSetter<TProperty>(Expression<Func<TEntity, TProperty>> expression) {
        var memberExpression = (MemberExpression)expression.Body;
        var instanceParameter = Expression.Parameter(typeof(TEntity), "instance");
        var valueParameter = Expression.Parameter(typeof(object), "value");

        // Cast the "value" object to the correct type.
        var castValue = Expression.Convert(valueParameter, typeof(TProperty));

        // Assign the value to the property.
        var assign = Expression.Assign(
            Expression.MakeMemberAccess(instanceParameter, memberExpression.Member),
            castValue
        );

        return Expression.Lambda<Action<TEntity, object?>>(
            assign,
            instanceParameter,
            valueParameter
        ).Compile();
    }
}