using System.Reflection;

namespace Nameless.NHibernate.Options;

public abstract record NHibernateOptionsBase {
    #region Public Methods

    public IEnumerable<KeyValuePair<string, string>> GetConfigValues() {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  .Where(property => !typeof(NHibernateOptionsBase)                                                 .IsAssignableFrom(property.PropertyType));

        foreach (var property in properties) {
            var key = property.GetDescription();
            var obj = property.GetValue(this);
            if (obj is null) { continue; }

            var value = obj is Enum @enum
                ? @enum.GetDescription()
                : obj.ToString();

            if (value is null) { continue; }

            yield return new KeyValuePair<string, string>(key, value);
        }
    }

    #endregion
}