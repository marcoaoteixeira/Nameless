using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Web.Identity.Entities;

namespace Nameless.Web.Identity;

public static class HostApplicationBuilderExtensions {
    public static THostApplicationBuilder RegisterIdentityServices<THostApplicationBuilder>(
        this THostApplicationBuilder self, Action<IdentityOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .AddIdentityCore<User>(configure ?? (_ => { }))
            .AddRoles<Role>();

        return self;
    }
}