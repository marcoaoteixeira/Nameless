using System;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;

namespace Nameless.Orm.NHibernate {
    public abstract class DirectiveBase<TResult> : IDirective<TResult> {
        #region Private Read-Only Fields

        private readonly ISession _session;

        #endregion

        #region Protected Constructors

        protected DirectiveBase (ISession session) {
            Prevent.ParameterNull (session, nameof (session));

            _session = session;
        }

        #endregion

        #region Protected Abstract Methods

        protected abstract Task<TResult> ExecuteAsync (ISession session, NameValueParameterSet parameters, IProgress<int> progress, CancellationToken token = default);

        #endregion

        #region IDirective<TResult> Members

        public Task<TResult> ExecuteAsync (NameValueParameterSet parameters, IProgress<int> progress = null, CancellationToken token = default) {
            return ExecuteAsync (_session, parameters, progress ?? NullProgress.Instance, token);
        }

        #endregion
    }
}