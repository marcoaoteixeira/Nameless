using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nameless.Web.Identity.Api.Inputs;
using Nameless.Web.Identity.Api.Outputs;
using Nameless.Web.Identity.Api.Requests;
using Nameless.Web.Infrastructure;

namespace Nameless.Web.Identity.Api.Endpoints {
    public sealed class GetUser : IMinimalEndpoint {
        #region Private Read-Only Fields

        private readonly IdentityApiOptions _options;

        #endregion

        #region Public Constructors

        public GetUser(IdentityApiOptions options) {
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region Private Static Methods

        private static async Task<IResult> HandleAsync(
            [AsParameters] GetUserInput input,
            IMediator mediator,
            CancellationToken cancellationToken) {
            var request = new AuthenticateUserRequest {
                UserName = input.Username ?? string.Empty,
                Password = input.Username ?? string.Empty
            };

            var response = await mediator.Send(request, cancellationToken);

            var output = new AuthenticateUserOutput {
                Token = response.Token,
                Error = response.Error,
                Succeeded = response.Succeeded()
            };

            return Results.Ok(output);
        }

        #endregion

        #region IMinimalEndpoint Members

        public string Name => "users";

        public string Summary => Constants.Endpoints
                                          .Summaries
                                          .GET_USER;

        public string Description => Constants.Endpoints
                                              .Descriptions
                                              .GET_USER;

        public string Group => Constants.Endpoints
                                        .Groups
                                        .USERS;

        public int Version => 1;

        public IEndpointConventionBuilder Map(IEndpointRouteBuilder builder)
            => builder.MapGet($"{_options.BaseUrl}/{Name}", HandleAsync)
                      .RequireAuthorization()
                      .Produces<GetUserOutput>()
                      .ProducesProblem(StatusCodes.Status401Unauthorized)
                      .ProducesProblem(StatusCodes.Status500InternalServerError);

        #endregion
    }
}
