using Nameless.EventSourcing.Stores.NHibernate.Models;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nameless.EventSourcing.Stores.NHibernate.Mappings {
    public sealed class SnapshotClassMapping : ClassMapping<SnapshotEntity> {
        #region Public Constructors

        public SnapshotClassMapping () {
            Table ("snapshots");

            Id (prop => prop.AggregateID, mapping => {
                mapping.Column ("event_id");
                mapping.Type (NHibernateUtil.Guid);
                mapping.Generator (Generators.GuidComb);
            });

            Property (prop => prop.Version, mapping => {
                mapping.Column ("version");
                mapping.Type (NHibernateUtil.Int32);
            });

            Property (prop => prop.Type, mapping => {
                mapping.Column ("type");
                mapping.Type (NHibernateUtil.String);
                mapping.Length (1024);
            });

            Property (prop => prop.Payload, mapping => {
                mapping.Column ("payload");
                mapping.Type (NHibernateUtil.Binary);
            });
        }

        #endregion
    }
}