using System.Reflection;

namespace Nameless.Validation.Abstractions {
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ValidateAttribute : Attribute {
        #region Public Static Methods

        public static bool Present(object? obj)
            => obj?.GetType()
                  .GetCustomAttribute<ValidateAttribute>(inherit: false) is not null;

        #endregion
    }
}
