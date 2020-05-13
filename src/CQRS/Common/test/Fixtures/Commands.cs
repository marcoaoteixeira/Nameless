using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.CQRS.Common.Test.Fixtures {
    public class SumCommand : ICommand {
        public int X { get; set; }
        public int Y { get; set; }

        public int Result { get; set; }
    }

    public class SumCommandHandler : ICommandHandler<SumCommand> {
        public Task HandleAsync (SumCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            command.Result = command.X + command.Y;
            return Task.CompletedTask;
        }
    }

    public class SubtractCommand : ICommand {
        public int X { get; set; }
        public int Y { get; set; }

        public int Result { get; set; }
    }

    public class SubtractCommandHandler : ICommandHandler<SubtractCommand> {
        public Task HandleAsync (SubtractCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            command.Result = command.X - command.Y;
            return Task.CompletedTask;
        }
    }
}