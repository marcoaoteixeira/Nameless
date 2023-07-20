using System.Collections;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nameless.AspNetCore {

    public sealed record BadRequestEntry {
        #region Public Properties

        [JsonPropertyName("code")]
        public string Code { get; init; } = null!;

        [JsonPropertyName("problems")]
        public string[] Problems { get; init; } = null!;

        #endregion
    }

    public sealed class BadRequestOutput : IEnumerable<BadRequestEntry> {
        #region Private Properties

        private List<BadRequestEntry> Bag { get; } = new();

        #endregion

        #region Public Methods

        public void Push(string code, string[] problems) {
            Prevent.Against.Null(code, nameof(code));
            Prevent.Against.Null(problems, nameof(problems));

            Bag.Add(new() {
                Code = code,
                Problems = problems
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

        IEnumerator<BadRequestEntry> IEnumerable<BadRequestEntry>.GetEnumerator()
            => Bag.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Bag.GetEnumerator();

        #endregion
    }

    public static class ModelStateDictionaryExtension {
        #region Public Static Methods

        public static BadRequestOutput CreateBadRequestOutput(this ModelStateDictionary self) {
            var result = new BadRequestOutput();

            foreach (var kvp in self) {
                var code = kvp.Key;
                var problems = new List<string>();
                foreach (var error in kvp.Value.Errors) {
                    var problem = string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? error.Exception?.Message ?? string.Empty
                        : string.Empty;

                    problems.Add(problem);
                }
                result.Push(code, problems.ToArray());
            }

            return result;
        }

        #endregion
    }
}
