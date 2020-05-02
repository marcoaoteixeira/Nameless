using System.Linq;
using Castle.DynamicProxy;
using NHibernate;

namespace Nameless.Orm.NHibernate {
    public sealed class EntityOwnerTrimmingSessionInterceptor : ISessionInterceptor {
        #region Public Static Read-Only Fields

        public static readonly string[] InterceptedMethods = new[] {
            nameof (ISession.Get),
            nameof (ISession.GetAsync),
            nameof (ISession.Load),
            nameof (ISession.LoadAsync),
            nameof (ISession.Query)
        };

        #endregion

        #region Private Read-Only Fields

        private readonly IInterceptorContext _context;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="EntityOwnerTrimmingSessionInterceptor"/>;
        /// </summary>
        /// <param name="context">The application context.</param>
        public EntityOwnerTrimmingSessionInterceptor (IInterceptorContext context) {
            Prevent.ParameterNull (context, nameof (context));

            _context = context;
        }

        #endregion

        #region IInterceptor Members

        public void Intercept (IInvocation invocation) {
            var applicationContext = _context as ApplicationContext;

            var proceed = applicationContext != null && (
                applicationContext.IsSystemAdministrator ||
                !typeof (ISession).IsAssignableFrom (invocation.InvocationTarget.GetType ()) ||
                !InterceptedMethods.Contains (invocation.Method.Name)
            );

            if (proceed) { invocation.Proceed (); return; }

            var session = invocation.InvocationTarget as ISession;
            session
                .EnableFilter (EntityOwnerTrimmingFilterDefinitionPolicy.FilterName)
                .SetParameter (EntityOwnerTrimmingFilterDefinitionPolicy.OwnerParameterName, applicationContext.Owner);

            invocation.Proceed ();
        }

        #endregion
    }
}
