using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.AspNetCore.Identity;
using Nameless.CQRS;

namespace Nameless.Skeleton.Web.Queries.Identity {
    public sealed class GetExternalLoginInfoQuery : IQuery<ExternalLoginInfo> {

    }

    public sealed class GetExternalLoginInfoQueryHandler : IQueryHandler<GetExternalLoginInfoQuery, ExternalLoginInfo> {
        #region Private Read-Only Fields

        private readonly SignInManager<User> _signInManager;

        #endregion

        #region Public Constructors

        public GetExternalLoginInfoQueryHandler (SignInManager<User> signInManager) {
            Prevent.ParameterNull (signInManager, nameof (signInManager));

            _signInManager = signInManager;
        }

        #endregion

        #region IQueryHandler<GetExternalLoginInfoQuery, ExternalLoginInfo>

        public async Task<ExternalLoginInfo> HandleAsync (GetExternalLoginInfoQuery query, IProgress<int> progress = null, CancellationToken token = default) {
            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync ();

            return externalLoginInfo;
        }

        #endregion
    }
}