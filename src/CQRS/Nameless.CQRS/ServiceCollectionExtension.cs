using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.CQRS;

public record CqrsOptions { }

public static class ServiceCollectionExtension {
    public static IServiceCollection RegisterCQRS(this IServiceCollection self,
                                                  Assembly[] supportAssemblies) {
        
        return self;
    }
}
