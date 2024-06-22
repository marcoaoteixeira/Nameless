using System.Diagnostics.CodeAnalysis;

namespace Nameless.Web.Identity.Api.Responses {
    public abstract record ResponseBase {
        #region Public Properties

        public string? Error { get; init; }

        #endregion

        #region Public Methods

        [MemberNotNullWhen(returnValue: false, nameof(Error))]
        public bool Succeeded()
            => string.IsNullOrWhiteSpace(Error);

        #endregion
    }
}
