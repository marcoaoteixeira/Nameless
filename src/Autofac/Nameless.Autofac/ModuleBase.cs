using System.Reflection;

namespace Nameless.Autofac {

    public abstract class ModuleBase : global::Autofac.Module {

        #region Protected Properties

        /// <summary>
        /// Gets the support assemblies.
        /// </summary>
        protected IEnumerable<Assembly> SupportAssemblies { get; }

        #endregion

        #region Protected Constructors

        /// <summary>
        /// Protected constructor.
        /// </summary>
        /// <param name="supportAssemblies">The support assemblies.</param>
        protected ModuleBase(params Assembly[] supportAssemblies) {
            SupportAssemblies = supportAssemblies ?? Array.Empty<Assembly>();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Searches for an implementation for a given service type.
        /// </summary>
        /// <typeparam name="TType">The service type.</typeparam>
        /// <returns>The service type implementation</returns>
        /// <exception cref="InvalidOperationException">If more that one implementation is found.</exception>
        protected Type? SearchForImplementation<TType>() {
            return SearchForImplementation(typeof(TType));
        }

        /// <summary>
        /// Searches for implementations for a given service type.
        /// </summary>
        /// <typeparam name="TType">The service type.</typeparam>
        /// <returns>An array of types</returns>
        protected Type[] SearchForImplementations<TType>() {
            return SearchForImplementations(typeof(TType));
        }

        /// <summary>
        /// Searches for an implementation for a given service type.
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <returns>The service type implementation</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">If more that one implementation is found.</exception>
        protected Type? SearchForImplementation(Type serviceType) {
            return SearchForImplementations(serviceType).SingleOrDefault();
        }

        /// <summary>
        /// Searches for implementations for a given service type.
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType" /> is <see langword="null" />.</exception>
        /// <returns>An array of types</returns>
        protected Type[] SearchForImplementations(Type serviceType) {
            Prevent.Null(serviceType, nameof(serviceType));

            if (!SupportAssemblies.Any()) { return Enumerable.Empty<Type>().ToArray(); }

            var result = SupportAssemblies
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => !type.IsInterface)
                .Where(type => !type.IsAbstract)
                .Where(type => !type.IsSingleton())
                .Where(type => serviceType.IsAssignableFrom(type) || serviceType.IsAssignableFromGenericType(type));

            return result.ToArray();
        }

        #endregion
    }
}
