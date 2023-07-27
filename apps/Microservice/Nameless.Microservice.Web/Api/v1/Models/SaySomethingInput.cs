using System.Text.Json.Serialization;
using FluentValidation;
using Nameless.FluentValidation;

namespace Nameless.Microservice.Web.Api.v1.Models {
    [FluentValidate]
    public sealed record SaySomethingInput {
        #region Public Properties

        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;

        #endregion
    }

    public sealed class SaySomethingInputValidator : AbstractValidator<SaySomethingInput> {
        public SaySomethingInputValidator() {
            RuleFor(_ => _.Message).NotEmpty();
        }
    }
}
