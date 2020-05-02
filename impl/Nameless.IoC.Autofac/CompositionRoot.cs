using System;
using System.Linq;
using Autofac;

namespace Nameless.IoC.Autofac {

    /// <summary>
    /// Default implementation of <see cref="ICompositionRoot"/> using Autofac (https://autofac.org/).
    /// </summary>
    public sealed class CompositionRoot : ICompositionRoot, IDisposable {

        #region Private Fields

        private bool _disposed;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Autofac container.
        /// </summary>
        public IContainer Container { get; private set; }

        #endregion

        #region Private Properties

        private ContainerBuilder Builder { get; set; }

        #endregion

        #region Destructor

        /// <summary>
        /// Destructor.
        /// </summary>
        ~CompositionRoot () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Methods

        private ContainerBuilder GetBuilder () {
            return Builder ?? (Builder = new ContainerBuilder ());
        }

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) { TearDown (); }
            _disposed = true;
        }

        #endregion

        #region ICompositionRoot Members

        /// <inheritdoc/>
        public IServiceResolver GetServiceResolver () {
            if (Container == null) {
                throw new InvalidOperationException ($"Composition root not started. Need to call method: {nameof (StartUp)}");
            }
            return new ServiceResolver (Container);
        }

        /// <inheritdoc/>
        public IServiceResolver GetScopedServiceResolver (params IServiceRegistration[] registrations) {
            if (Container == null) {
                throw new InvalidOperationException ($"Composition root not started. Need to call method: {nameof (StartUp)}");
            }
            return new ServiceResolver (Container.BeginLifetimeScope (builder => {
                registrations.Each (registration => builder.RegisterModule ((Module)registration));
            }));
        }

        /// <inheritdoc/>
        public void Compose (params IServiceRegistration[] registrations) {
            if (Container != null) {
                throw new InvalidOperationException ("Composition root already started.");
            }

            if (!registrations.Any (_ => typeof (ServiceRegistrationBase).IsAssignableFrom (_.GetType ()))) {
                throw new InvalidOperationException ($"All services registrations must inherit {nameof (ServiceRegistrationBase)}.");
            }

            registrations.Each (_ => GetBuilder ().RegisterModule ((Module)_));
        }

        /// <inheritdoc/>
        public void StartUp () {
            if (Container != null) {
                throw new InvalidOperationException ("Composition root already started.");
            }
            Container = GetBuilder ().Build ();
        }

        /// <inheritdoc/>
        public void TearDown () {
            if (Container != null) {
                Container.Dispose ();
            }
            Container = null;
            Builder = null;
        }

        #endregion

        #region IDisposable Members

        /// <inheritdoc/>
        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (obj: this);
        }

        #endregion
    }
}