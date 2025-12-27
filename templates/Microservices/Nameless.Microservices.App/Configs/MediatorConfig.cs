using System.Reflection;
using Nameless.Mediator;
using Nameless.Microservices.App.Infrastructure.Mediator;

namespace Nameless.Microservices.App.Configs;

public static class MediatorConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureMediator(Assembly[] assemblies) {
            // Configures the Mediator service with the specified assemblies.
            // Mediator pattern (https://en.wikipedia.org/wiki/Mediator_pattern) is
            // a design pattern that allows for loose coupling between components.
            // Note: We could use MediatR, but we decided to implement our own Mediator service
            // since it going to be a commercial licensed product from version 14 onwards.
            self.Services.RegisterMediator(options => {
                options.Assemblies = assemblies;

                // This pipeline behavior (open-generic) will be applied
                // to every request handler that implements IRequestHandler.
                // Under the hood, it uses FluentValidation to validate requests,
                // so you need to create the corresponding validators for your requests.
                // The pipeline depends on the IValidationService to work properly.
                options.RegisterRequestPipelineBehavior(typeof(ValidateRequestPipelineBehavior<,>));
            });

            return self;
        }
    }
}
