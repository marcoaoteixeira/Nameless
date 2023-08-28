using FluentValidation;
using Nameless.DailyDos.Web.Domain.Requests;

namespace Nameless.DailyDos.Web.Domain.Validators {
    public sealed class CreateTodoItemRequestValidator : AbstractValidator<CreateTodoItemRequest> {
        #region Public Constructors

        public CreateTodoItemRequestValidator() {
            RuleFor(_ => _.Description)
                .NotEmpty();
        }

        #endregion
    }
}
