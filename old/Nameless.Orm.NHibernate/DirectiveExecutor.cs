using System;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;

namespace Nameless.Orm.NHibernate {
    public sealed class DirectiveExecutor : IDirectiveExecutor {
        #region Private Read-Only Fields

        private readonly ISession _session;

        #endregion

        #region Public Constructors

        public DirectiveExecutor (ISession session) {
            Prevent.ParameterNull (session, nameof (session));

            _session = session;
        }

        #endregion

        #region IDirectiveExecutor Members

        public Task<TResult> ExecuteDirectiveAsync<TResult, TDirective> (NameValueParameterSet parameters, IProgress<int> progress = null, CancellationToken token = default) where TDirective : IDirective<TResult> {
            if (!typeof (DirectiveBase<>).IsAssignableFrom (typeof (TDirective))) {
                throw new InvalidOperationException ($"Directive must inherit from \"{typeof (DirectiveBase<>)}\"");
            }

            var directive = (IDirective<TResult>)Activator.CreateInstance (typeof (TDirective), new object[] { _session });
            return directive.ExecuteAsync (parameters, progress ?? NullProgress.Instance, token);
        }

        #endregion
    }
}