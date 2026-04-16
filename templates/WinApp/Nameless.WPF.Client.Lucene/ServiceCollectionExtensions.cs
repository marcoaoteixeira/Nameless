using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Lucene;
using Nameless.Lucene.Repository;

namespace Nameless.WPF.Client.Lucene;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterLucene(Assembly[] assemblies) {
            self.RegisterLucene(
                registration => registration.IncludeAssemblies(assemblies),
                opts => opts.DirectoryName = LuceneConstants.DatabaseDirectoryName
            );

            self.RegisterLuceneRepository(
                registration => registration.IncludeAssemblies(assemblies)
            );

            return self;
        }
    }
}
