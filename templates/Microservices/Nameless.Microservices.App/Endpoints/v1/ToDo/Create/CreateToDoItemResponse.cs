using Nameless.Microservices.App.Entities;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Microservices.App.Endpoints.v1.ToDo.Create;

public sealed class CreateToDoItemResponse : Result<ToDoItem> {
    private CreateToDoItemResponse(ToDoItem? value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator CreateToDoItemResponse(ToDoItem value) {
        return new CreateToDoItemResponse(value, errors: []);
    }

    public static implicit operator CreateToDoItemResponse(Error error) {
        return new CreateToDoItemResponse(null, errors: [error]);
    }

    public static implicit operator CreateToDoItemResponse(Error[] errors) {
        return new CreateToDoItemResponse(null, errors);
    }
}