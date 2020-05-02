using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Nameless.Orm.EntityFrameworkCore {
    public class DirectiveExecutor : IDirectiveExecutor {
        #region Private Properties

        private DbContext Context { get; }

        #endregion

        #region Public Constructors

        public DirectiveExecutor (DbContext context) {
            Prevent.ParameterNull (context, nameof (context));

            Context = context;
        }

        #endregion

        #region IDirectiveExecutor Members

        public Task<TResult> ExecuteDirectiveAsync<TResult, TDirective> (NameValueParameterSet parameters, IProgress<int> progress = null, CancellationToken token = default) where TDirective : IDirective<TResult> {
            if (!typeof (DirectiveBase<>).GetTypeInfo ().IsAssignableFrom (typeof (TDirective))) {
                throw new InvalidOperationException ($"Directive must inherit from \"{typeof (DirectiveBase<>)}\"");
            }

            var directive = (IDirective<TResult>)Activator.CreateInstance (typeof (TDirective), new object[] { Context });
            return directive.ExecuteAsync (parameters, progress ?? NullProgress.Instance, token);
        }

        #endregion
    }
}