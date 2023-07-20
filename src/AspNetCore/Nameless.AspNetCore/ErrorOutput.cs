using System.Collections;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nameless.AspNetCore {

    public sealed record Error {
        #region Public Properties

        [JsonPropertyName("code")]
        public string Code { get; init; } = null!;

        [JsonPropertyName("problems")]
        public string[] Problems { get; init; } = null!;

        #endregion
    }

    public sealed class ErrorOutput : IEnumerable<Error> {
        #region Private Properties

        private List<Error> Bag { get; } = new();

        #endregion

        #region Public Methods

        public void Push(string code, string[] messages) {
            Prevent.Against.Null(code, nameof(code));
            Prevent.Against.Null(messages, nameof(messages));

            Bag.Add(new() {
                Code = code,
                Problems = messages
            });
        }

        public void Push(Exception ex) {
            Prevent.Against.Null(ex, nameof(ex));

            Bag.Add(new() {
                Code = ex.Message,
                Problems = ex.StackTrace?.Split(Environment.NewLine) ?? Array.Empty<string>()
            });
        }

        #endregion

        #region IEnumerable<Error> Members

        IEnumerator<Error> IEnumerable<Error>.GetEnumerator()
            => Bag.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Bag.GetEnumerator();

        #endregion
    }

    public static class ModelStateDictionaryExtension {
        #region Public Static Methods

        public static ErrorOutput CreateErrorOutput(this ModelStateDictionary self) {
            var errorOutput = new ErrorOutput();

            foreach (var kvp in self) {
                var code = kvp.Key;
                var messages = new List<string>();
                foreach (var error in kvp.Value.Errors) {
                    var message = string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? error.Exception?.Message ?? string.Empty
                        : string.Empty;

                    messages.Add(message);
                }
                errorOutput.Push(code, messages.ToArray());
            }

            return errorOutput;
        }

        #endregion
    }
}
