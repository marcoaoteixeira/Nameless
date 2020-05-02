using System;
using System.Linq;
using Castle.DynamicProxy;
using Nameless.Helpers;
using NHibernate;

namespace Nameless.Orm.NHibernate {
    public sealed class EntityWriterSessionInterceptor : ISessionInterceptor {
        #region Public Static Read-Only Fields

        public static readonly string[] InterceptedMethods = new[] {
            nameof (ISession.Save),
            nameof (ISession.SaveAsync),
            nameof (ISession.SaveOrUpdate),
            nameof (ISession.SaveOrUpdateAsync),
            nameof (ISession.Update),
            nameof (ISession.UpdateAsync),
            nameof (ISession.Persist),
            nameof (ISession.PersistAsync),
        };

        #endregion

        #region Private Read-Only Fields

        private readonly IInterceptorContext _context;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="EntityWriterSessionInterceptor"/>;
        /// </summary>
        /// <param name="context">The application context.</param>
        public EntityWriterSessionInterceptor (IInterceptorContext context) {
            Prevent.ParameterNull (context, nameof (context));

            _context = context;
        }

        #endregion

        #region IInterceptor Members

        public void Intercept (IInvocation invocation) {
            var applicationContext = _context as ApplicationContext;

            if (!InterceptedMethods.Contains (invocation.Method.Name)) { invocation.Proceed (); return; }

            var entity = invocation.Arguments.SingleOrDefault (_ => typeof (EntityBase).IsAssignableFrom (_.GetType ())) as EntityBase;
            if (entity != null) {
                if (entity.ID == Guid.Empty) {
                    ReflectionHelper.SetPrivateFieldValue (entity, "_id", Guid.NewGuid ());
                }

                if (entity.CreationDate == DateTime.MinValue) {
                    ReflectionHelper.SetPrivateFieldValue (entity, "_creationDate", DateTime.UtcNow);
                }

                ReflectionHelper.SetPrivateFieldValue (entity, "_modificationDate", DateTime.UtcNow);

                if (entity.Owner == Guid.Empty) {
                    ReflectionHelper.SetPrivateFieldValue (entity, "_owner", applicationContext.Owner);
                }
            }

            invocation.Proceed ();
        }

        #endregion
    }
}