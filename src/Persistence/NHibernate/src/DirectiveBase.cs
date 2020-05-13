using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.Persistence.NHibernate {
    public abstract class DirectiveBase<TResult> : IDirective<TResult> {
        #region Protected Properties

        protected global::NHibernate.ISession Session { get; }

        #endregion

        #region Protected Constructors

        protected DirectiveBase (global::NHibernate.ISession session) {
            Prevent.ParameterNull (session, nameof (session));

            Session = session;
        }

        #endregion

        #region IDirective<TResult> Members

        public abstract Task<TResult> ExecuteAsync (NameValueParameterSet parameters, IProgress<int> progress = null, CancellationToken token = default);

        #endregion
    }
}