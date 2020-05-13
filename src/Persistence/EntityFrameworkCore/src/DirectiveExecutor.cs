using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Nameless.Persistence.EntityFrameworkCore {
    public class DirectiveExecutor : IDirectiveExecutor {
        #region Private Read-Only Fields

        private readonly DbContext _context;

        #endregion

        #region Public Constructors

        public DirectiveExecutor (DbContext context) {
            Prevent.ParameterNull (context, nameof (context));

            _context = context;
        }

        #endregion

        #region IDirectiveExecutor Members

        public Task<TResult> ExecuteDirectiveAsync<TResult, TDirective> (NameValueParameterSet parameters, IProgress<int> progress = null, CancellationToken token = default) where TDirective : IDirective<TResult> {
            if (!typeof (TDirective).IsAssignableToGenericType (typeof (DirectiveBase<>))) {
                throw new InvalidOperationException ($"Directive must inherit from \"{typeof (DirectiveBase<>)}\"");
            }

            var directive = (IDirective<TResult>) Activator.CreateInstance (typeof (TDirective), new object[] { _context });
            return directive.ExecuteAsync (parameters, progress ?? NullProgress.Instance, token);
        }

        #endregion
    }
}