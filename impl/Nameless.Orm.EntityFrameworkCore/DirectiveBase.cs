using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Nameless.Orm.EntityFrameworkCore {
    public abstract class DirectiveBase<TEntity> : IDirective<TEntity> where TEntity : class {
        #region Protected Properties

        protected DbContext Context { get; }

        #endregion

        #region Protected Constructors

        protected DirectiveBase (DbContext context) {
            Prevent.ParameterNull (context, nameof (context));

            Context = context;
        }

        #endregion

        #region IDirective<TEntity> Members

        public abstract Task<TEntity> ExecuteAsync (NameValueParameterSet parameters, IProgress<int> progress = null, CancellationToken token = default);

        #endregion
    }
}