using System;
using System.Linq;
using System.Reflection;
using Nameless.Persistence.Properties;

namespace Nameless.Persistence {

    /// <summary>
    /// Attributes that defines if the property or field will be an entity ID.
    /// </summary>
    [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class IDAttribute : Attribute {

        #region Private Static Read-Only Fields

        private static readonly string[] Conventions = { "ID", "Id", "_id", "id" };

        #endregion

        #region Public Static Methods

        public static string GetName<T> () {
            return GetName (typeof (T));
        }

        public static string GetName (Type type) {
            var properties = type
                .GetProperties ()
                .Where (property => property.GetCustomAttribute<IDAttribute> (inherit: true) != null)
                .ToArray ();

            return properties.Length > 0 ? properties[0].Name : Conventions[0];
        }

        /// <summary>
        /// Extract the ID from the object passed as parameter.
        /// </summary>
        /// <param name="obj">The instance of the object.</param>
        /// <returns>The eligible ID from the object.</returns>
        /// <remarks>
        /// Eligible ID's are those members who are instance and public or non-public (private, protected, internal)
        /// properties or fields. Includes readonly properties or fields.
        /// </remarks>
        public static object GetValue (object obj) {
            var type = obj.GetType ();
            var members = type
                .GetMembers (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where (member =>
                    (member.MemberType == MemberTypes.Field || (member.MemberType == MemberTypes.Property && ((PropertyInfo) member).CanRead)) &&
                    (member.GetCustomAttribute<IDAttribute> (inherit: true) != null || Conventions.Any (convention => member.Name == convention))
                ).OrderByDescending (member =>
                    member.GetCustomAttribute<IDAttribute> (inherit: true) != null
                ).ToArray ();

            // Ok, no eligible members, sorry, not doughnuts for you.
            if (members.Length == 0) {
                throw new InvalidOperationException (string.Format (Resources.ID_Member_Not_Found_MessageFormatter, type, string.Join (", ", Conventions)));
            }

            return members[0].MemberType == MemberTypes.Property ?
                ((PropertyInfo) members[0]).GetValue (obj, null) :
                ((FieldInfo) members[0]).GetValue (obj);
        }

        #endregion
    }
}