using Microsoft.Extensions.Configuration;
using Nameless.Attributes;

namespace Nameless;

/// <summary>
///     <see cref="IConfiguration"/> extension methods.
/// </summary>
public static class ConfigurationExtensions {
    extension(IConfiguration self) {
        /// <summary>
        ///     Retrieves the configuration section related to the
        ///     <typeparamref name="TOptions"/> type. To discover
        ///     the section name, it looks for the attribute
        ///     <see cref="ConfigurationSectionNameAttribute"/> in the type.
        /// </summary>
        /// <typeparam name="TOptions">
        ///     Type which is attributed to the configuration section.
        /// </typeparam>
        /// <param name="sectionName">
        ///     The section name.
        /// </param>
        /// <returns>
        ///     The configuration section for the type or an empty section;
        ///     if not found.
        /// </returns>
        public IConfigurationSection GetSection<TOptions>(string? sectionName = null) {
            sectionName ??= ConfigurationSectionNameAttribute.GetSectionName<TOptions>();

            if (self is IConfigurationSection section && section.Key == sectionName) {
                return section;
            }

            return self.GetSection(sectionName);
        }

        /// <summary>
        ///     Retrieves a POCO representing an option object from the
        ///     configuration.
        /// </summary>
        /// <typeparam name="TOptions">
        ///     Type of the options.
        /// </typeparam>
        /// <param name="sectionName">
        ///     When provided, use it to locate the corresponding configuration
        ///     section. Otherwise; it tries to get the configuration section
        ///     name from the type using
        ///     the <see cref="ConfigurationSectionNameAttribute"/>.
        /// </param>
        /// <returns>
        ///     The options object.
        /// </returns>
        public TOptions GetOptions<TOptions>(string? sectionName = null)
            where TOptions : new() {
            return self.GetSection<TOptions>(sectionName)
                       .Get<TOptions>() ?? new TOptions();
        }

        /// <summary>
        ///     Retrieves all options from a configuration section looking
        ///     into its children.
        /// </summary>
        /// <typeparam name="TOptions">
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
        ///     of <typeparamref name="TOptions"/>
        /// </exception>
        public Dictionary<string, TOptions> GetMultipleOptions<TOptions>(string? sectionName = null) {
            return self.GetSection<TOptions>(sectionName)
                       .GetChildren()
                       .ToDictionary(
                           section => section.Key,
                           section => section.Get<TOptions>() ?? throw new InvalidOperationException($"Unable to convert configuration section '{sectionName}' to '{typeof(TOptions).Name}'.")
                       );
        }
    }
}
