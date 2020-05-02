using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.AspNetCore.Identity {
    public static class IdentityBuilderExtension {
        #region Public Static Methods

        public static IdentityBuilder AddIdentitySimpleDataStorage (this IdentityBuilder builder) {
            builder.Services.AddScoped<IUserStore<User>, UserStore> ();
            builder.Services.AddScoped<IRoleStore<Role>, RoleStore> ();
            return builder;
        }

        #endregion
    }
}