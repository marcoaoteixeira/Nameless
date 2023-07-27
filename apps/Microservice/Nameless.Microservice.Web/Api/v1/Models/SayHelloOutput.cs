namespace Nameless.Microservice.Web.Api.v1.Models {
    public sealed record SayHelloOutput {
        #region Public Properties

        public string Message { get; init; } = null!;

        #endregion
    }
}
