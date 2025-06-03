using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Testing.Tools.Mockers;
using Nameless.Web.Identity.Mockers;

namespace Nameless.Web.Identity.Fakes;

public class FakeUserManager : UserManager<User> {
    public FakeUserManager(IUserStore<User> store = null,
                           IOptions<Microsoft.AspNetCore.Identity.IdentityOptions> options = null,
                           IPasswordHasher<User> passwordHasher = null,
                           IEnumerable<IUserValidator<User>> userValidators = null,
                           IEnumerable<IPasswordValidator<User>> passwordValidators = null,
                           ILookupNormalizer keyNormalizer = null,
                           IdentityErrorDescriber errors = null,
                           IServiceProvider services = null,
                           ILogger<UserManager<User>> logger = null)
        : base(store ?? new UserStoreMocker().Build(),
            options ?? new OptionsMocker<Microsoft.AspNetCore.Identity.IdentityOptions>().Build(),
            passwordHasher ?? new PasswordHasherMocker().Build(),
            userValidators ?? [],
            passwordValidators ?? [],
            keyNormalizer ?? new LookupNormalizerMocker().Build(),
            errors ?? new FakeIdentityErrorDescriber(),
            services ?? new ServiceProviderMocker().Build(),
            logger ?? new LoggerMocker<UserManager<User>>().Build()) {
    }
}