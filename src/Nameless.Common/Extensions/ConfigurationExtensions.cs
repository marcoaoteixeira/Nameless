using Microsoft.Extensions.Configuration;
using Nameless.Configuration;

namespace Nameless;

/// <summary>
///     <see cref="IConfiguration"/> extension methods.
/// </summary>
public static class ConfigurationExtensions {
    extension(IConfiguration self) {
        /// <summary>
        ///     Retrieves the configuration section related to the
        ///     <typeparamref name="T"/> type. To discover
        ///     the section name, it looks for the attribute
        ///     <see cref="ConfigurationSectionNameAttribute"/> in the type.
        /// </summary>
        /// <typeparam name="T">
        ///     Type which is attributed to the configuration section.
        /// </typeparam>
        /// <param name="sectionName">
        ///     The configuration section name. If not provided, tries to
        ///     check if <typeparamref name="T"/> has attribute
        ///     <see cref="ConfigurationSectionNameAttribute"/>. If not,
        ///     defaults to the <typeparamref name="T"/> type name.
        /// </param>
        /// <returns>
        ///     The configuration section for the type or an empty section;
        ///     if not found.
        /// </returns>
        public IConfigurationSection GetSection<T>(string? sectionName = null) {
            sectionName ??= ConfigurationSectionNameAttribute.GetSectionName<T>();

            if (self is IConfigurationSection section && section.Key == sectionName) {
                return section;
            }

            return self.GetSection(sectionName);
        }

        /// <summary>
        ///     Tries to get an instance of <typeparamref name="T"/> from the
        ///     current configuration section. If unable to bind, then throws
        ///     <see cref="ConfigurationBindingException"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     Type to bind.
        /// </typeparam>
        /// <returns>
        ///     An instance of <typeparamref name="T"/> that reflects the
        ///     configuration section.
        /// </returns>
        /// <exception cref="ConfigurationBindingException">
        ///     If not able to bind configuration section to
        ///     <typeparamref name="T"/>
        /// </exception>
        public T GetOrThrow<T>() where T : class {
            return self.Get<T>() ?? throw new ConfigurationBindingException(
                path: ((IConfigurationSection)self).Path,
                bindingType: typeof(T)
            );
        }

        /// <summary>
        ///     Gets an instance of <typeparamref name="T"/> from the
        ///     current configuration section. If unable to bind, then creates
        ///     a new instance and uses the <paramref name="configure"/>
        ///     delegate, if provided.
        /// </summary>
        /// <typeparam name="T">
        ///     Type to bind.
        /// </typeparam>
        /// <returns>
        ///     An instance of <typeparamref name="T"/> that reflects the
        ///     configuration section.
        /// </returns>
        public T GetOrCreate<T>(Action<T>? configure = null) where T : class, new() {
            var instance = self.Get<T>() ?? new T();

            configure?.Invoke(instance);

            return instance;
        }

        /// <summary>
        ///     Retrieves all options from a configuration section looking
        ///     into its children.
        /// </summary>
        /// <typeparam name="T">
        ///     Type of the option.
        /// </typeparam>
        /// <param name="sectionName">
        ///     The section name. If not provided, will try to get it from
        ///     the option type.
        /// </param>
        /// <returns>
        ///     A dictionary containing all options and section names.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     If it was not able to convert the section into an instance
        ///     of <typeparamref name="T"/>
        /// </exception>
        public Dictionary<string, T> GetAll<T>(string? sectionName = null) where T : class {
            return self.GetSection<T>(sectionName)
                       .GetChildren()
                       .ToDictionary(
                           section => section.Key,
                           section => section.GetOrThrow<T>()
                       );
        }
    }
}
