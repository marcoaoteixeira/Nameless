using Microsoft.AspNetCore.Mvc;
using Nameless.Mediator;
using Nameless.ObjectModel;
using Nameless.Validation;
using Nameless.Web;
using Nameless.Web.Endpoints;
using Nameless.Web.Endpoints.Definitions;

namespace Nameless.Microservices.App.Endpoints.v1.ToDo.Create;

public sealed class CreateToDoItemEndpoint : IEndpoint {
    private readonly IMediator _mediator;
    private readonly IValidationService _validationService;

    public CreateToDoItemEndpoint(IMediator mediator, IValidationService validationService) {
        _mediator = mediator;
        _validationService = validationService;
    }

    public IEndpointDescriptor Describe() {
        return EndpointDescriptorBuilder.Create<CreateToDoItemEndpoint>()
                                        .Post("/", nameof(HandleAsync))
                                        .AllowAnonymous()
                                        .WithGroupName("todo")
                                        .WithSummary("Create New To-Do Item")
                                        .WithDescription("Creates a new To-Do item in your list of chores.")
                                        .Accepts<CreateToDoItemInput>()
                                        .Produces<CreateToDoItemOutput>()
                                        .ProducesProblem()
                                        .ProducesValidationProblem()
                                        .Build();
    }

    public async Task<IResult> HandleAsync([FromBody] CreateToDoItemInput input, CancellationToken cancellationToken = default) {
        var validation = await _validationService.ValidateAsync(input, cancellationToken);
        if (!validation.Succeeded) {
            return TypedResults.ValidationProblem(validation.ToDictionary());
        }

        var request = new CreateToDoItemRequest(input.Summary ?? string.Empty, input.DueDate);
        var response = await _mediator.ExecuteAsync(request, cancellationToken);

        return response.Match(
            onSuccess: value => TypedResults.Ok(new CreateToDoItemOutput(value.ID, value.Summary, value.DueDate)),
            onFailure: IResult (errors) => {
                if (errors.All(error => error.Type == ErrorType.Validation)) {
                    return TypedResults.ValidationProblem(errors.ToValidationProblemDictionary());
                }

                return TypedResults.Problem(errors.ToProblemDetails());
            }
        );
    }
}