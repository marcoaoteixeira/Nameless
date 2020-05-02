using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nameless.Properties;

namespace Nameless.Helpers {

    /// <summary>
    /// Reflection helper.
    /// </summary>
    public static class ReflectionHelper {

        #region Public Static Methods

        /// <summary>
        /// Searchs, recursively, and retrieves the value of a private read-only field.
        /// </summary>
        /// <param name="instance">The <see cref="object"/> instance where the field belongs.</param>
        /// <param name="name">The name of the field.</param>
        /// <returns>The value of the field.</returns>
        public static object GetPrivateFieldValue (object instance, string name) {
            Prevent.ParameterNull (instance, nameof (instance));
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));

            var field = GetPrivateField (instance.GetType (), name);

            if (field == null) {
                throw new FieldAccessException (string.Format (Resources.FieldNotFound, name));
            }

            return field.GetValue (instance);
        }

        /// <summary>
        /// Searchs, recursively, and sets the value of a private read-only field.
        /// </summary>
        /// <param name="instance">The <see cref="object"/> instance where the field belongs.</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="value">The new value.</param>
        public static void SetPrivateFieldValue (object instance, string name, object value) {
            Prevent.ParameterNull (instance, nameof (instance));
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));

            var field = GetPrivateField (instance.GetType (), name);

            if (field == null) {
                throw new FieldAccessException (string.Format (Resources.FieldNotFound, name));
            }

            field.SetValue (instance, value);
        }

        public static IEnumerable<MethodInfo> GetMethodsBySignature (Type type, Type returnType = null, Type customAttributeType = null, bool matchParameterInheritance = true, params Type[] parameterTypes) {
            Prevent.ParameterNull (type, nameof (type));

            return type.GetRuntimeMethods ().Where (method => {
                var currentReturnType = returnType ?? typeof (void);
                if (method.ReturnType != currentReturnType) { return false; }

                if (customAttributeType != null && method.GetCustomAttributes (customAttributeType, inherit : true).Any () == false) {
                    return false;
                }

                var parameters = method.GetParameters ();
                var currentParameterTypes = (parameterTypes ?? Enumerable.Empty<Type> ()).ToArray ();
                if (parameters.Length != currentParameterTypes.Length) { return false; }

                return currentParameterTypes.Select ((parameterType, index) => {
                    var match = parameters[index].ParameterType == parameterType;
                    var assignable = parameterType.IsAssignableFrom (parameters[index].ParameterType);

                    return match || (assignable && matchParameterInheritance);
                }).All (result => result == true);
            });
        }

        #endregion Public Static Methods

        #region Private Static Methods

        private static FieldInfo GetPrivateField (Type type, string name) {
            var result = type.GetTypeInfo ().GetField (name, BindingFlags.Instance | BindingFlags.NonPublic);

            if (result == null && type.GetTypeInfo ().BaseType != null) {
                return GetPrivateField (type.GetTypeInfo ().BaseType, name);
            }

            return result;
        }

        #endregion Private Static Methods
    }
}