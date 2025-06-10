using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web;

public static class ServiceCollectionExtensions {
    public static IServiceCollection RegisterJsonWebTokenServices(this IServiceCollection self) {
        self
           .AddAuthentication(options => {

           })
           .AddJwtBearer(options => {

           });

        return self;
    }
}
