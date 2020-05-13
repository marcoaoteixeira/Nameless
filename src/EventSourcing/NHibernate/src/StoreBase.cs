using Nameless.Persistence;
using Nameless.Serialization;

namespace Nameless.EventSourcing.Stores.NHibernate {
    public abstract class StoreBase {

        #region Protected Properties

        protected IRepository Repository { get; }
        protected ISerializer Serializer { get; }

        #endregion

        #region Protected Constructors

        public StoreBase (IRepository repository, ISerializer serializer) {
            Prevent.ParameterNull (repository, nameof (repository));
            Prevent.ParameterNull (serializer, nameof (serializer));

            Repository = repository;
            Serializer = serializer;
        }

        #endregion
    }
}