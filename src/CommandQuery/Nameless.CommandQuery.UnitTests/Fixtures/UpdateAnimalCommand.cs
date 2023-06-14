using AutoMapper;
using FluentValidation;
using Nameless.Infrastructure;

namespace Nameless.CommandQuery.UnitTests.Fixtures {

    public sealed class UpdateAnimalCommand : ICommand {

        public int ID { get; set; }
        public string? Name { get; set; }
    }

    public sealed class UpdateAnimalCommandHandler : CommandHandlerBase<UpdateAnimalCommand> {
        public UpdateAnimalCommandHandler(IMapper mapper, IValidator<UpdateAnimalCommand>? validator = null)
            : base(mapper, validator) { }

        protected override Task<ExecutionResult> InnerHandleAsync(UpdateAnimalCommand command, CancellationToken cancellationToken = default) {
            return Task.FromResult(ExecutionResult.Successful(command.ID));
        }
    }
}
