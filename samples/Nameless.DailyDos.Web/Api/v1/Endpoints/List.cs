using Nameless.DailyDos.Web.Api.v1.Models.Input;
using Nameless.DailyDos.Web.Api.v1.Models.Output;
using Nameless.Web.Infrastructure;

namespace Nameless.DailyDos.Web.Api.v1.Endpoints {
    public sealed class List : IMinimalEndpoint {
        #region Public Static Methods

        public static IResult Handle([AsParameters] ListTodoItemInput query) {
            return Results.Ok(null);
        }

        #endregion

        #region IMinimalEndpoint Members

        public string Name => nameof(List);

        public string Summary => "Get To-Do's";

        public string Description => "Get To-Do's";

        public string Group => "To-Do";

        public int Version => 1;

        IEndpointConventionBuilder IMinimalEndpoint.Map(IEndpointRouteBuilder builder)
            => builder
                .MapGet($"{Root.Endpoints.BASE_API_PATH}/todo", Handle)

                .Produces<TodoItemOutput[]>();

        #endregion
    }
}
