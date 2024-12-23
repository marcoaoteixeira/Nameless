using System.Reflection;

namespace Nameless.Helpers;

/// <summary>
/// Reflection helper.
/// </summary>
public static class ReflectionHelper {
    /// <summary>
    /// Retrieves all methods from a type.
    /// </summary>
    /// <param name="type">The type to look upon.</param>
    /// <param name="returnType">The expected method return type. If <c>null</c> then matches any <c>void</c>.</param>
    /// <param name="matchParameterInheritance">If the method parameters matches the types in <paramref name="parameterTypes"/>.</param>
    /// <param name="parameterTypes">The method parameters type.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> object, where T is <see cref="MethodInfo"/>.</returns>
    public static IEnumerable<MethodInfo> GetMethodsBySignature(Type type, Type? returnType = null, bool matchParameterInheritance = true, params Type[] parameterTypes)
        => Prevent.Argument
                  .Null(type)
                  .GetRuntimeMethods()
                  .Where(method => {
                      if (method.ReturnType != (returnType ?? typeof(void))) {
                          return false;
                      }

                      var parameters = method.GetParameters();
                      var filter = CreateParameterFilter(matchParameterInheritance, parameters);

                      return parameterTypes.Select(filter).All(result => result);
                  });

    private static Func<Type, int, bool> CreateParameterFilter(bool matchParameterInheritance, ParameterInfo[] parameters)
        => (parameterType, index) => {
            var match = parameters[index].ParameterType == parameterType;
            var assignable = parameterType.IsAssignableFrom(parameters[index].ParameterType);
            return match || (assignable && matchParameterInheritance);
        };
}