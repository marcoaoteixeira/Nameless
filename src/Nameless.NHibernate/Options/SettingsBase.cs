using System.Reflection;

namespace Nameless.NHibernate.Options;

/// <summary>
/// Base class for all NHibernate settings.
/// </summary>
public abstract record SettingsBase {
    /// <summary>
    /// Retrieves all settings as a collection of <see cref="KeyValuePair{TKey,TValue}"/>
    /// </summary>
    /// <returns></returns>
    public IEnumerable<KeyValuePair<string, string>> GetConfigValues() {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  .Where(property => !typeof(SettingsBase).IsAssignableFrom(property.PropertyType));

        foreach (var property in properties) {
            var propDescription = property.GetDescription(false);
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