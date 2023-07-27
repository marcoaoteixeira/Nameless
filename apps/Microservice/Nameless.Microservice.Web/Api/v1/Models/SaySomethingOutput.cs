namespace Nameless.Microservice.Web.Api.v1.Models {
    public sealed record SaySomethingOutput {
        #region Public Properties

        public string Message { get; init; } = null!;

        #endregion
    }
}
