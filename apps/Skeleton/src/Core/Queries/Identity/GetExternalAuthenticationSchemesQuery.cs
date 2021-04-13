using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Nameless.AspNetCore.Identity;
using Nameless.CQRS;

namespace Nameless.Skeleton.Web.Queries.Identity {
    public sealed class GetExternalAuthenticationSchemesQuery : IQuery<IEnumerable<AuthenticationScheme>> {

    }

    public sealed class GetExternalAuthenticationSchemesQueryHandler : IQueryHandler<GetExternalAuthenticationSchemesQuery, IEnumerable<AuthenticationScheme>> {
        #region Private Read-Only Fields

        private readonly SignInManager<User> _signInManager;

        #endregion

        #region Public Constructors

        public GetExternalAuthenticationSchemesQueryHandler (SignInManager<User> signInManager) {
            Prevent.ParameterNull (signInManager, nameof (signInManager));

            _signInManager = signInManager;
        }

        #endregion

        #region IQueryHandler<GetExternalAuthenticationSchemesQuery, IEnumerable<AuthenticationScheme>>

        public async Task<IEnumerable<AuthenticationScheme>> HandleAsync (GetExternalAuthenticationSchemesQuery query, IProgress<int> progress = null, CancellationToken token = default) {
            var externalSchemes = await _signInManager.GetExternalAuthenticationSchemesAsync ();

            return externalSchemes;
        }

        #endregion
    }
}