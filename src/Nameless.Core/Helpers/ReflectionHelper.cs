﻿using System.Reflection;

namespace Nameless.Helpers;

/// <summary>
/// Reflection helper.
/// </summary>
public static class ReflectionHelper {
    /// <summary>
    /// Searches, recursively, and retrieves the value of a private read-only field.
    /// </summary>
    /// <param name="instance">The <see cref="object"/> instance where the field belongs.</param>
    /// <param name="name">The name of the field.</param>
    /// <returns>The value of the field.</returns>
    public static object? GetPrivateFieldValue(object instance, string name) {
        Prevent.Argument.Null(instance);
        Prevent.Argument.NullOrWhiteSpace(name);

        var field = GetPrivateField(instance.GetType(), name);

        return field is not null
            ? field.GetValue(instance)
            : throw new FieldAccessException($"Field \"{name}\" not found.");
    }

    /// <summary>
    /// Searches, recursively, and sets the value of a private read-only field.
    /// </summary>
    /// <param name="instance">The <see cref="object"/> instance where the field belongs.</param>
    /// <param name="name">The name of the field.</param>
    /// <param name="value">The new value.</param>
    public static void SetPrivateFieldValue(object instance, string name, object value) {
        Prevent.Argument.Null(instance);
        Prevent.Argument.NullOrWhiteSpace(name);

        var field = GetPrivateField(instance.GetType(), name)
                 ?? throw new FieldAccessException($"Field \"{name}\" not found.");

        field.SetValue(instance, value);
    }

    /// <summary>
    /// Retrieves all methods by a specific signature.
    /// </summary>
    /// <param name="type">The type that will be looked up.</param>
    /// <param name="returnType">The method return type.</param>
    /// <param name="methodAttributeType">The method attribute type, if exists.</param>
    /// <param name="matchParameterInheritance">If it matches parameter inheritance.</param>
    /// <param name="parameterTypes">The method parameters type.</param>
    /// <returns>An <see cref="IEnumerable{MethodInfo}"/> with all found methods.</returns>
    public static IEnumerable<MethodInfo> GetMethodsBySignature(Type type, Type? returnType = null, Type? methodAttributeType = null, bool matchParameterInheritance = true, params Type[] parameterTypes) {
        Prevent.Argument.Null(type);

        return type
               .GetRuntimeMethods()
               .Where(method => {
                   if (method.ReturnType != (returnType ?? typeof(void))) {
                       return false;
                   }

                   if (methodAttributeType is not null && method.GetCustomAttributes(methodAttributeType, inherit: true)
                                                                .Length == 0) {
                       return false;
                   }

                   var parameters = method.GetParameters();

                   return parameterTypes
                          .Select((parameterType, index) => {
                              var match = parameters[index].ParameterType == parameterType;
                              var assignable = parameterType.IsAssignableFrom(parameters[index].ParameterType);
                              return match || (assignable && matchParameterInheritance);
                          })
                          .All(result => result);
               });
    }

    private static FieldInfo? GetPrivateField(Type type, string name) {
        while (true) {
            var result = type.GetField(name,
                                       bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

            if (result is not null || type.BaseType is null) {
                return result;
            }

            type = type.BaseType;
        }
    }
}