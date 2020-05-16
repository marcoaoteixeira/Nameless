using System;
using System.Threading.Tasks;
using Autofac;
using Nameless.CQRS.Common.Test.Fixtures;
using Nameless.DependencyInjection.Autofac;
using Xunit;

namespace Nameless.CQRS.Common.Test {
    public class CommandQueryDispatcherTest : IDisposable {
        private IContainer _container;
        public void Dispose () {
            if (_container != null) {
                _container.Dispose ();
            }
        }

        public CommandQueryDispatcherTest () {
            var supportAssemblies = new [] { typeof (CommandQueryDispatcherTest).Assembly };
            var builder = new ContainerBuilder ();
            builder.RegisterModule (new CQRSModule (supportAssemblies));
            _container = builder.Build ();
        }

        [Fact]
        public async Task CommandAsync_WithSumCommand_ReturnsCorrectValue () {
            // arrange
            ICommandQueryDispatcher dispatcher = new CommandQueryDispatcher (new ServiceResolver (_container));
            SumCommand command = new SumCommand { X = 10, Y = 40 };

            // act
            await dispatcher.CommandAsync (command);

            // assert
            Assert.Equal (50, command.Result);
        }
    }
}