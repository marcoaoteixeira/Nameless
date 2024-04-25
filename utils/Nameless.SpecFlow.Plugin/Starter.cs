using Nameless.SpecFlow.Plugin;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly:GeneratorPlugin(typeof(Starter))]

namespace Nameless.SpecFlow.Plugin {
    public sealed class Starter : IGeneratorPlugin {
        public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
            => generatorPluginEvents.RegisterDependencies += RegisterDependencies;

        private void RegisterDependencies(object? sender, RegisterDependenciesEventArgs args) {
            args.ObjectContainer.RegisterTypeAs<CategoryTestClassTagDecorator, ITestClassTagDecorator>(CategoryTestClassTagDecorator.TAG_NAME);
            args.ObjectContainer.RegisterTypeAs<CategoryTestMethodTagDecorator, ITestMethodTagDecorator>(CategoryTestMethodTagDecorator.TAG_NAME);
            args.ObjectContainer.RegisterTypeAs<MyMethodTagDecorator, ITestMethodTagDecorator>(MyMethodTagDecorator.TAG_NAME);
        }
    }
}
