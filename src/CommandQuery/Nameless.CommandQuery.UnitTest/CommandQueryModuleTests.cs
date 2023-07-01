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

            if (builder != null) {
                builder(containerBuilder);
            }

            return containerBuilder.Build();
        }

        [Test]
        public void CommandQueryModule_Resolve_CommandDispatcher() {
            // arrange
            using var container = CreateContainer(builder => {
                var mapper = Substitute.For<IMapper>();

                builder.RegisterInstance(mapper);
            });

            // act
            var dispatcher = container.Resolve<ICommandService>();

            // assert
            Assert.That(dispatcher, Is.Not.Null);
        }

        [Test]
        public void CommandQueryModule_Resolve_QueryDispatcher() {
            // arrange
            using var container = CreateContainer(builder => {
                var mapper = Substitute.For<IMapper>();

                builder.RegisterInstance(mapper);
            });

            // act
            var dispatcher = container.Resolve<IQueryService>();

            // assert
            Assert.That(dispatcher, Is.Not.Null);
        }

        [Test]
        public void CommandDispatcher_Execute_Command_Simple() {
            // arrange
            using var container = CreateContainer(builder => {
                var mapper = Substitute.For<IMapper>();

                builder.RegisterInstance(mapper);
            });
            var service = container.Resolve<ICommandService>();

            // act && assert
            Assert.DoesNotThrowAsync(async () => await service.ExecuteAsync(new SaveAnimalCommand { Name = "Test" }));
        }

        [Test]
        public void CommandDispatcher_Execute_Command_Inherited() {
            // arrange
            using var container = CreateContainer(builder => {
                var mapper = Substitute.For<IMapper>();

                builder.RegisterInstance(mapper);
            });
            var service = container.Resolve<ICommandService>();

            // act && assert
            Assert.DoesNotThrowAsync(async () => await service.ExecuteAsync(new UpdateAnimalCommand { ID = 2, Name = "Test" }));
        }
    }
}