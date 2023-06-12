using System.Diagnostics.CodeAnalysis;

namespace Nameless.Helpers {

    public static class IDHelper {

        #region Public Static Methods

        public static bool TryGetAs<T>(object? value, [NotNullWhen(true)] out T? output) {
            var result = TryGetAs(value, typeof(T), out object? obj);

            output = result ? (T)obj! : default;

            return result;
        }

        public static bool TryGetAs(object? value, Type convertType, [NotNullWhen(true)] out object? output) {
            output = default;

            if (value is null) { return false; }

            // Strange outcome, but OK
            if (convertType == typeof(object)) {
                output = value;
                return true;
            }

            if (convertType == typeof(Guid) && Guid.TryParse(value.ToString(), out Guid guid)) {
                output = guid;
                return true;
            }

            if (typeof(IConvertible).IsAssignableFrom(value.GetType())) {
                try { output = Convert.ChangeType(value, convertType); } catch { return false; }

                return true;
            }

            return false;
        }

        #endregion
    }
}
