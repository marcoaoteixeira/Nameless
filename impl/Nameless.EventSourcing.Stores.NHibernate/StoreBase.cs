using Nameless.Serialization;
using NHibernate;

namespace Nameless.EventSourcing.Stores.NHibernate {
    public abstract class StoreBase {
        
        #region Protected Properties

        protected ISession Session { get; }
        protected ISerializer Serializer { get; }

        #endregion

        #region Protected Constructors

        public StoreBase (ISession session, ISerializer serializer) {
            Prevent.ParameterNull (session, nameof (session));
            Prevent.ParameterNull (serializer, nameof (serializer));

            Session = session;
            Serializer = serializer;
        }

        #endregion
    }
}