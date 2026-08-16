using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Web;

public static class ResultExtensions {
    extension<T>(Result<T> self) {
        public IResult ToHttpResult() {
            return self.Match(
                onSuccess: TypedResults.Ok,
                onFailure: ToFailureResult
            );
        }
    }

    private static IResult ToFailureResult(Error[] errors) {
        if (errors.All(error => error.Type is ErrorType.Missing)) {
            return TypedResults.NotFound(string.Join("; ", errors.Select(error => error.Message)));
        }

        if (errors.All(error => error.Type is ErrorType.Forbidden)) {
            return TypedResults.Forbid();
        }

        if (errors.All(error => error.Type is ErrorType.Unauthorized)) {
            return TypedResults.Unauthorized();
        }

        if (errors.All(error => error.Type is ErrorType.Validation)) {
            return ToValidationProblem(errors);
        }

        return ToProblemHttpResult(errors);
    }

    private static ValidationProblem ToValidationProblem(Error[] errors) {
        return TypedResults.ValidationProblem(errors.ToDictionary());
    }

    private static ProblemHttpResult ToProblemHttpResult(Error[] errors) {
        return TypedResults.Problem(
            new ProblemDetails {
                Detail = string.Join("; ", errors.Select(error => error.Message))
            }
        );
    }
}
