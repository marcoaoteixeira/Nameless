using Nameless.Sample.Chores.Repositories;
using Nameless.Sample.Chores.Services;
using Nameless.Validation.FluentValidation;

namespace Nameless.Sample.Chores.Configs;

public static class AppConfig {
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection self) {
        return self
              .ConfigureValidationServices(configure => {
                  configure.Assemblies = [typeof(AppConfig).Assembly];
              })
              .AddSingleton<IChoreService, ChoreService>()
              .AddSingleton<IChoreRepository, ChoreRepository>();
    }
}
