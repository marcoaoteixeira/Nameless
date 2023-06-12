using System.Runtime.Serialization;

namespace Nameless.Autofac {


    [Serializable]
    public class ModuleException : Exception {

        #region Public Constructors

        public ModuleException() { }

        public ModuleException(string message)
            : base(message) { }

        public ModuleException(string message, Exception inner)
            : base(message, inner) { }

        #endregion

        #region Protected Constructors

        protected ModuleException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        #endregion
    }
}
