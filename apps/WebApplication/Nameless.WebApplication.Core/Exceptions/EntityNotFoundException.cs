using System.Runtime.Serialization;

namespace Nameless.WebApplication {


    [Serializable]
    public class EntityNotFoundException : Exception {

        #region Public Constructors

        public EntityNotFoundException(Type entityType)
            : this($"Entity {entityType.Name} not found.") { }
        
        public EntityNotFoundException(string message)
            : base(message) { }

        public EntityNotFoundException(string message, Exception inner)
            : base(message, inner) { }

        #endregion

        #region Protected Constructors

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        #endregion
    }
}
