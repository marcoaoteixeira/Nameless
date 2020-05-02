namespace Nameless.IoC {

    /// <summary>
    /// Composition root interface.
    /// </summary>
    public interface ICompositionRoot {

        #region Methods

        /// <summary>
        /// Composes the root.
        /// </summary>
        /// <param name="registrations">Registrations.</param>
        void Compose (params IServiceRegistration[] registrations);

        /// <summary>
        /// Start up the composition root.
        /// </summary>
        void StartUp ();

        /// <summary>
        /// Tears down the composition root.
        /// </summary>
        void TearDown ();

        /// <summary>
        /// Retrieves the current resolver.
        /// </summary>
        /// <returns>An instance of <see cref="IServiceResolver"/>.</returns>
        IServiceResolver GetServiceResolver ();

        /// <summary>
        /// Retrieves a scoped resolver, that can receive new registrations.
        /// </summary>
        /// <param name="registrations">The registrations.</param>
        /// <returns>An instance of <see cref="IServiceResolver"/>.</returns>
        IServiceResolver GetScopedServiceResolver (params IServiceRegistration[] registrations);

        #endregion Methods
    }
}