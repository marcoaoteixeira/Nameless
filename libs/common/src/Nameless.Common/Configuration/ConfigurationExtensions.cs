using Microsoft.Extensions.Configuration;

namespace Nameless.Configuration;

/// <summary>
///     <see cref="IConfiguration"/> extension methods.
/// </summary>
public static class ConfigurationExtensions {
    extension(IConfiguration self) {
        /// <summary>
        ///     Gets the configuration section associated with type
        ///     <typeparamref name="T"/>, returning the current configuration
        ///     itself when it already is that section.
        /// </summary>
        /// <typeparam name="T">
        ///     The type whose section name is resolved through
        ///     <see cref="ConfigurationSectionNameAttribute"/> when
        ///     <paramref name="sectionName"/> is not provided.
        /// </typeparam>
        /// <param name="sectionName">
        ///     The name of the section to look for. When
        ///     <see langword="null"/>, the name is taken from the
        ///     <see cref="ConfigurationSectionNameAttribute"/> applied to
        ///     <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        ///     The current <see cref="IConfiguration"/> if it is an
        ///     <see cref="IConfigurationSection"/> whose
        ///     <see cref="IConfigurationSection.Key"/> matches the section
        ///     name (case-insensitive); otherwise the child section with
        ///     that name. The returned section is never
        ///     <see langword="null"/>, but it may not exist; use
        ///     <c>Exists()</c> to check.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <b>Caveat:</b> the "is this already the section?" check
        ///         compares only the section
        ///         <see cref="IConfigurationSection.Key"/> (the last path
        ///         segment), not its <see cref="IConfigurationSection.Path"/>.
        ///         If the current section is named <c>Foo</c> and also has
        ///         a child named <c>Foo</c>, the current (parent) section is
        ///         returned, not the child (<c>Foo:Foo</c>). This differs from
        ///         <see cref="IConfiguration.GetSection(string)"/>, which
        ///         always resolves relative to the current node.
        ///     </para>
        ///     <para>
        ///         The match is case-insensitive, consistent with how
        ///         configuration keys are compared. A
        ///         <paramref name="sectionName"/> containing the key delimiter
        ///         (<c>:</c>) never matches the
        ///         <see cref="IConfigurationSection.Key"/>, so it always falls
        ///         through to <see cref="IConfiguration.GetSection(string)"/>.
        ///     </para>
        /// </remarks>
        public IConfigurationSection GetSectionFor<T>(string? sectionName = null) {
            sectionName ??= ConfigurationSectionNameAttribute.GetSectionName<T>();

            if (self is IConfigurationSection section && string.Equals(section.Key, sectionName, StringComparison.OrdinalIgnoreCase)) {
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
            return self.GetSectionFor<T>(sectionName)
                       .GetChildren()
                       .ToDictionary(
                           section => section.Key,
                           section => section.GetOrThrow<T>()
                       );
        }
    }
}
