namespace Nameless.ProducerConsumer.RabbitMQ {
    internal static class ParameterHelper {
        #region Internal Static Methods

        internal static IDictionary<string, object> ToDictionary(IEnumerable<Parameter> parameters) {
            var dictionary = new Dictionary<string, object>();
            foreach (var parameter in parameters) {
                dictionary.TryAdd(parameter.Name, parameter.Value);
            }
            return dictionary;
        }

        #endregion
    }
}
