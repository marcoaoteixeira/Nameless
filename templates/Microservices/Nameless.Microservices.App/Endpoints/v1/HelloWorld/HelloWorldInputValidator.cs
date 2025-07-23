using FluentValidation;

namespace Nameless.Microservices.App.Endpoints.v1.HelloWorld;

public class HelloWorldInputValidator : AbstractValidator<HelloWorldInput> {
    public HelloWorldInputValidator() {
        RuleFor(input => input.Person)
            .NotEmpty()
            .WithMessage("Person cannot be empty.");
    }
}