using System.Reflection;

namespace Nameless.Persistence {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class IDAttribute : Attribute {

        #region Public Static Read-Only Properties

        /// <summary>
        /// Gets the convention fields for identifiers.
        /// Values are "_id" and "id".
        /// </summary>
        public static string[] ConventionFields => new[] { "_id", "id" };
        /// <summary>
        /// Gets the convention properties for identifiers.
        /// Values are "Id" and "ID".
        /// </summary>
        public static string[] ConventionProperties => new[] { "Id", "ID" };

        #endregion

        #region Public Static Methods

        public static ID GetID<T>() => GetID(typeof(T));

        public static ID GetID<T>(T instance) => GetID(typeof(T), instance);

        public static ID GetID(Type type, object? instance = null) {
            Prevent.Null(type, nameof(type));

            MemberInfo? member;

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            member = fields.FirstOrDefault(_ => _.GetCustomAttribute<IDAttribute>() != null)
                ?? fields.FirstOrDefault(_ => ConventionFields.Contains(_.Name));

            if (member != null) {
                return new(member.Name, instance != null ? ((FieldInfo)member).GetValue(instance) : null);
            }

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            member = properties.FirstOrDefault(_ => _.GetCustomAttribute<IDAttribute>() != null)
                ?? properties.FirstOrDefault(_ => ConventionProperties.Contains(_.Name));

            if (member != null) {
                return new(member.Name, instance != null ? ((PropertyInfo)member).GetValue(instance) : null);
            }

            return ID.Empty;
        }

        #endregion
    }

    public record ID {
        #region Public Static Read-Only Properties

        public static ID Empty => new(nameof(ID));

        #endregion

        #region Public Properties

        public string Name { get; }
        public object? Value { get; }

        #endregion

        #region Public Constructors

        public ID(string name, object? value = null) {
            Prevent.NullOrWhiteSpaces(name, nameof(name));

            Name = name;
            Value = value;
        }

        #endregion

        #region Public Override Methods

        public override string ToString() {
            return $"[{(Value != null ? Value.GetType().Name : "null")}] {Name}: {Value}";
        }

        #endregion
    }
}
