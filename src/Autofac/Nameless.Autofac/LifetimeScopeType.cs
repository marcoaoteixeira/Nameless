namespace Nameless.Autofac {

    /// <summary>
    /// Life time scope types.
    /// </summary>
    public enum LifetimeScopeType : int {

        /// <summary>
        /// Singleton life time.
        /// </summary>
        Singleton,

        /// <summary>
        /// Life time "per request".
        /// </summary>
        PerScope,

        /// <summary>
        /// Every request to the object gets a new instance.
        /// </summary>
        Transient
    }
}