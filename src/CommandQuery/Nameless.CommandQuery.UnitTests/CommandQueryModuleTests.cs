using Autofac;
using AutoMapper;
using Nameless.CommandQuery.UnitTests.Fixtures;
using NSubstitute;

namespace Nameless.CommandQuery.UnitTests {
    public class CommandQueryModuleTests {

        private static IContainer CreateContainer(Action<ContainerBuilder>? builder = null) {
            var containerBuilder = new ContainerBuilder();

            containerBuilder
                .RegisterModule(new CommandQueryModule(typeof(CommandQueryModuleTests).Assembly));

            if (builder != default) {
                builder(containerBuilder);
            }

            return containerBuilder.Build();
        }

        [Test]
        public void CommandQueryModule_Resolve_DispatcherService() {
            // arrange
            using var container = CreateContainer(builder => {
                var mapper = Substitute.For<IMapper>();

                builder.RegisterInstance(mapper);
            });

            // act
            var dispatcherService = container.Resolve<IDispatcherService>();

            // assert
            Assert.That(dispatcherService, Is.Not.Null);
        }

        [Test]
        public async Task DispatcherService_Execute_Command_Simple() {
            // arrange
            using var container = CreateContainer(builder => {
                var mapper = Substitute.For<IMapper>();

                builder.RegisterInstance(mapper);
            });
            var dispatcherService = container.Resolve<IDispatcherService>();

            // act
            var response = await dispatcherService.ExecuteAsync(new SaveAnimalCommand { Name = "Test" });

            // assert
            Assert.Multiple(() => {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.True);
                Assert.That(response.State, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task DispatcherService_Execute_Command_Inherited() {
            // arrange
            using var container = CreateContainer(builder => {
                var mapper = Substitute.For<IMapper>();

                builder.RegisterInstance(mapper);
            });
            var dispatcherService = container.Resolve<IDispatcherService>();

            // act
            var response = await dispatcherService.ExecuteAsync(new UpdateAnimalCommand { ID = 2, Name = "Test" });

            // assert
            Assert.Multiple(() => {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.True);
                Assert.That(response.State, Is.EqualTo(2));
            });
        }
    }
}