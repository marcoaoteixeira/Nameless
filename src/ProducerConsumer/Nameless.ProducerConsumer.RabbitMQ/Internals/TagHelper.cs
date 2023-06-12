using System.Text;

namespace Nameless.PubSub.RabbitMQ {

    internal static class TagHelper {

        #region Internal Static Methods

        internal static string GenerateTag<T>(Action<T> action) {
            if (action == null) { return string.Empty; }

            var method = action.Method;
            var parameters = method.GetParameters().Select(_ => $"{_.ParameterType.Name} {_.Name}").ToArray();
            var signature = $"{method.DeclaringType?.FullName}.{method.Name}({string.Join(", ", parameters)})";
            var buffer = Encoding.UTF8.GetBytes(signature);

            return Convert.ToBase64String(buffer);
        }

        #endregion
    }
}
