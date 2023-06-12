using Newtonsoft.Json;

namespace Nameless.WebApplication.Domain {

    public sealed class ErrorOutput {

        #region Public Static Read-Only Properties

        public static ErrorOutput Empty => new();

        #endregion

        #region Public Properties

        [JsonProperty("errors")]
        public IDictionary<string, string[]> Errors { get; }

        #endregion

        #region Public Constructors

        public ErrorOutput(IDictionary<string, string[]>? errors = default) {
            Errors = errors ?? new Dictionary<string, string[]>();
        }

        #endregion
    }
}
