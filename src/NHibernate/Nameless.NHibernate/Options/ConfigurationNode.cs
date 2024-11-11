using System.Reflection;

namespace Nameless.NHibernate.Options;

public abstract record ConfigurationNode {
    public IEnumerable<KeyValuePair<string, string>> GetConfigValues() {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  .Where(property => !typeof(ConfigurationNode).IsAssignableFrom(property.PropertyType));

        foreach (var property in properties) {
            var propDescription = property.GetDescription(fallbackToName: false);
            if (string.IsNullOrWhiteSpace(propDescription)) { continue; }

            var propValue = property.GetValue(this);
            if (propValue is null) { continue; }

            var value = propValue is Enum @enum
                ? @enum.GetDescription()
                : propValue.ToString();

            if (value is null) { continue; }

            yield return new KeyValuePair<string, string>(propDescription, value);
        }
    }
}