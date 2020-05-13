using System;
using System.Collections.Generic;
using Autofac;
using Nameless.AspNetCore.Identity.NHibernate;
using Nameless.FileProvider;
using Nameless.Localization.Json;
using Nameless.Logging.Log4net;
using Nameless.Persistence.NHibernate;
using Nameless.Serialization.Json;

namespace Nameless.Skeleton.Web {
    public partial class StartUp {
        #region Public Methods

        public void ConfigureContainer (ContainerBuilder builder) {
            builder
                .RegisterModule (new FileProviderModule ())
                .RegisterModule (new LocalizationModule ())
                .RegisterModule (new LoggingModule ())
                .RegisterModule (new PersistenceModule {
                    Mappings = GetMappings ()
                })
                .RegisterModule (new SerializationModule ());
        }

        #endregion

        #region Private Static Methods

        private static IDictionary<Type, object[]> GetMappings () {
            return new Dictionary<Type, object[]> {
                // Identity objects
                { typeof (RoleClaimClassMapping), null },
                { typeof (RoleClassMapping), null },
                { typeof (UserClaimClassMapping), null },
                { typeof (UserClassMapping), null },
                { typeof (UserLoginClassMapping), null },
                { typeof (UserRoleClassMapping), null },
                { typeof (UserTokenClassMapping), null }
            };
        }

        #endregion
    }
}