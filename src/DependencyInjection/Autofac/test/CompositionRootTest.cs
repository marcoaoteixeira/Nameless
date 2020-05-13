using System;
using Autofac;
using Nameless.DependencyInjection;
using Xunit;

namespace Nameless.DependencyInjection.Autofac.Test {
    public class CompositionRootTest {
        [Fact]
        public void Compose_Start_Resolve_TearDown () {
            // arrange
            ICompositionRoot root = new CompositionRoot ();

            // act
            root.Compose (composer => {
                composer.AddTransient<IMathService, MathService> ();
            });
            root.StartUp ();
            var service = ((CompositionRoot) root).Container.Resolve<IMathService> ();
            var result = service.Sum (2, 2);

            // assert
            Assert.Equal (4, result);
        }

        [Fact]
        public void Compose_With_Modules () {
            // arrange
            ICompositionRoot root = new CompositionRoot ();

            // act
            root.Compose (composer => {
                composer.AddModule<MathServiceModule> ();
            });
            root.StartUp ();
            var service = ((CompositionRoot) root).Container.Resolve<IMathService> ();
            var result = service.Sum (2, 2);

            // assert
            Assert.Equal (4, result);
        }

        [Fact]
        public void Compose_With_Scopes () {
            // arrange
            ICompositionRoot root = new CompositionRoot ();

            // act
            root.Compose (composer => {
                composer.AddSingleton<IServiceResolver, ServiceResolver> (Parameter.Create (true));
                composer.AddModule<MathServiceModule> ();
            });
            root.StartUp ();
            var resolver = ((CompositionRoot) root).Container.Resolve<IServiceResolver> ();
            using (var scope = resolver.GetScoped (_ => _.AddTransient <IStringService, StringService> ())) {
                
            }

            var mathService = ((CompositionRoot) root).Container.Resolve<IMathService> ();
            var result = mathService.Sum (2, 2);

            // assert
            Assert.Equal (4, result);
        }
    }

    public interface IMathService {
        int Sum (int x, int y);
    }

    public class MathService : IMathService {
        public int Sum (int x, int y) => x + y;
    }

    public class MathServiceModule : ModuleBase {
        protected override void Load (ContainerBuilder builder) {
            builder.RegisterType<MathService> ().As<IMathService> ().InstancePerDependency ();
        }
    }

    public interface IStringService {
        string ToUpperCase (string value);
    }

    public class StringService : IStringService {
        public string ToUpperCase (string value) => (value ?? string.Empty).ToUpper ();
    }
}