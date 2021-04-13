namespace Nameless.Skeleton.Web.Models {
    public sealed class ErrorViewModel {
        #region Public Properties

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty (RequestId);

        #endregion
    }
}