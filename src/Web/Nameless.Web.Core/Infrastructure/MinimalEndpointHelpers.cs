using System.Reflection;

namespace Nameless.Web.Infrastructure {
    internal static class MinimalEndpointHelpers {
        #region Internal Static Methods

        /// <summary>
        /// Retrieves all implementations of <see cref="IMinimalEndpoint"/> from the assemblies marked to be scanned.
        /// </summary>
        /// <param name="assemblies">The assemblies that will be scanned.</param>
        /// <returns>A collection of implementations for <see cref="IMinimalEndpoint"/>.</returns>
        internal static IEnumerable<Type> ScanAssemblies(IEnumerable<Assembly> assemblies)
            => assemblies.SelectMany(type => type.ExportedTypes)
                         .Where(type => type is { IsInterface: false, IsAbstract: false } &&
                                        typeof(IMinimalEndpoint).IsAssignableFrom(type));

        #endregion
    }
}
