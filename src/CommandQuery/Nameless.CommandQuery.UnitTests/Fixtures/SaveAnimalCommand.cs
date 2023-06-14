using Nameless.Infrastructure;

namespace Nameless.CommandQuery.UnitTests.Fixtures {

    public sealed class SaveAnimalCommand : ICommand {

        public string Name { get; set; } = default!;
    }

    public sealed class SaveAnimalCommandHandler : ICommandHandler<SaveAnimalCommand> {

        public Task<ExecutionResult> HandleAsync(SaveAnimalCommand command, CancellationToken cancellationToken = default) {
            return Task.FromResult(ExecutionResult.Successful(1));
        }
    }
}
