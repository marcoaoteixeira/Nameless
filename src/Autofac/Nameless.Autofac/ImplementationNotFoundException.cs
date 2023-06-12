using System.Runtime.Serialization;

namespace Nameless.Autofac {


    [Serializable]
    public class ImplementationNotFoundException : Exception {

        #region Public Constructors

        public ImplementationNotFoundException(Type serviceType)
            : this($"Implementation for service type ${serviceType.FullName} not found.") { }

        public ImplementationNotFoundException(string message)
            : base(message) { }

        public ImplementationNotFoundException(string message, Exception inner)
            : base(message, inner) { }

        #endregion

        #region Protected Constructors

        protected ImplementationNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        #endregion
    }
}
