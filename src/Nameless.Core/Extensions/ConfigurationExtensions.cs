using Microsoft.Extensions.Configuration;
using Nameless.Attributes;

namespace Nameless;

/// <summary>
///     <see cref="IConfiguration"/> extension methods.
/// </summary>
public static class ConfigurationExtensions {
    extension(IConfiguration self) {
        /// <summary>
        ///     Whether the current <see cref="IConfiguration"/> is the root
        ///     configuration instance.
        /// </summary>
        public bool IsRoot => self is IConfigurationRoot;

        /// <summary>
        ///     Retrieves the configuration section related to the
        ///     <typeparamref name="TConfigurationSection"/> type. To discover
        ///     the section name, it looks for the attribute
        ///     <see cref="ConfigurationSectionNameAttribute"/> in the type.
        /// </summary>
        /// <typeparam name="TConfigurationSection">
        ///     Type which is attributed to the configuration section.
        /// </typeparam>
        /// <returns>
        ///     The configuration section for the type or an empty section;
        ///     if not found.
        /// </returns>
        public IConfigurationSection GetSection<TConfigurationSection>() {
            var sectionName = ConfigurationSectionNameAttribute.GetSectionName<TConfigurationSection>();

            if (self is IConfigurationSection section && section.Path == sectionName) {
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
            where TOptions : class, new() {
            sectionName ??= ConfigurationSectionNameAttribute.GetSectionName<TOptions>();
            
            return self.GetSection(sectionName).Get<TOptions>() ?? new TOptions();
        }
    }
}
