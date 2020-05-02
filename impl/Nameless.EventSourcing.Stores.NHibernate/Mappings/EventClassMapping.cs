using Nameless.EventSourcing.Stores.NHibernate.Models;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nameless.EventSourcing.Stores.NHibernate.Mappings {
    public sealed class EventClassMapping : ClassMapping<EventEntity> {
        #region Public Constructors

        public EventClassMapping () {
            Table ("events");

            Id (prop => prop.EventID, mapping => {
                mapping.Column ("event_id");
                mapping.Type (NHibernateUtil.Guid);
                mapping.Generator (Generators.GuidComb);
            });

            Property (prop => prop.AggregateID, mapping => {
                mapping.Column ("aggregate_id");
                mapping.Type (NHibernateUtil.Guid);
            });

            Property (prop => prop.Version, mapping => {
                mapping.Column ("version");
                mapping.Type (NHibernateUtil.Int32);
            });

            Property (prop => prop.TimeStamp, mapping => {
                mapping.Column ("time_stamp");
                mapping.Type (NHibernateUtil.DateTimeOffset);
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