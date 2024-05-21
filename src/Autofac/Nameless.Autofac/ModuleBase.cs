using System.Reflection;

namespace Nameless.Autofac {
    public abstract class ModuleBase : global::Autofac.Module {
        #region Protected Properties

        /// <summary>
        /// Gets the support assemblies.
        /// </summary>
        protected Assembly[] SupportAssemblies { get; }

        #endregion

        #region Protected Constructors

        /// <summary>
        /// Protected constructor.
        /// </summary>
        protected ModuleBase()
            : this([]) { }

        /// <summary>
        /// Protected constructor.
        /// </summary>
        /// <param name="supportAssemblies">The support assemblies.</param>
        protected ModuleBase(Assembly[] supportAssemblies) {
            SupportAssemblies = Guard.Against.Null(supportAssemblies, nameof(supportAssemblies));
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Retrieves, from support assemblies, a single implementation from the given service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>The service implementation <see cref="Type"/>.</returns>
        /// <exception cref="InvalidOperationException">If more than one implementation were found.</exception>
        protected Type? GetImplementation<TService>()
            => GetImplementations(typeof(TService)).SingleOrDefault();

        /// <summary>
        /// Retrieves, from support assemblies, a single implementation from the given service type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The service implementation <see cref="Type"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">If more than one implementation were found.</exception>
        protected Type? GetImplementation(Type serviceType)
            => GetImplementations(serviceType).SingleOrDefault();

        /// <summary>
        /// Retrieves, from support assemblies, all implementations from the given service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>An <see cref="IEnumerable{Type}"/> that contains all possible implementation types.</returns>
        protected IEnumerable<Type> GetImplementations<TService>()
            => GetImplementations(typeof(TService));

        /// <summary>
        /// Retrieves, from support assemblies, all implementations from the given service type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>An <see cref="IEnumerable{Type}"/> that contains all possible implementation types.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType"/> is <c>null</c>.</exception>
        protected IEnumerable<Type> GetImplementations(Type serviceType) {
            Guard.Against.Null(serviceType, nameof(serviceType));

            return SupportAssemblies
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => type is { IsInterface: false, IsAbstract: false } &&
                               !type.HasAttribute<SingletonAttribute>() &&
                               (serviceType.IsAssignableFrom(type) || serviceType.IsAssignableFromGenericType(type)));
        }

        #endregion
    }
}
