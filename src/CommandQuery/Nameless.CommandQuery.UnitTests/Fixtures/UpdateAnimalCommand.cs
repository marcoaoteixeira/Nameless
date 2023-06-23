using FluentValidation;

namespace Nameless.CommandQuery.UnitTests.Fixtures {
    public sealed class UpdateAnimalCommand : ICommand {
        public int ID { get; set; }
        public string? Name { get; set; }
    }

    public sealed class UpdateAnimalCommandHandler : CommandHandlerBase<UpdateAnimalCommand> {
        public UpdateAnimalCommandHandler(IValidator<UpdateAnimalCommand>? validator = null)
            : base(validator) { }

        protected override Task InnerHandleAsync(UpdateAnimalCommand command, CancellationToken cancellationToken = default) {
            return Task.CompletedTask;
        }
    }
}
