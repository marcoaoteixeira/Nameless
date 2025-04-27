using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Mockers;
using Nameless.Web.Identity.Mockers;

namespace Nameless.Web.Identity.Fakes;
public class FakeSignInManager : SignInManager<User> {
    private Func<string, string, bool, bool, Task<SignInResult>> _passwordSignInAsyncCallback;

    public FakeSignInManager(UserManager<User> userManager = null,
                             IHttpContextAccessor contextAccessor = null,
                             IUserClaimsPrincipalFactory<User> claimsFactory = null,
                             IOptions<Microsoft.AspNetCore.Identity.IdentityOptions> optionsAccessor = null,
                             ILogger<SignInManager<User>> logger = null,
                             IAuthenticationSchemeProvider schemes = null,
                             IUserConfirmation<User> confirmation = null)
        : base(userManager ?? new FakeUserManager(),
               contextAccessor ?? new HttpContextAccessorMocker().Build(),
               claimsFactory ?? new UserClaimsPrincipalFactoryMocker().Build(),
               optionsAccessor ?? new OptionsMocker<Microsoft.AspNetCore.Identity.IdentityOptions>().Build(),
               logger ?? new LoggerMocker<SignInManager<User>>().Build(),
               schemes ?? new AuthenticationSchemeProviderMocker().Build(),
               confirmation ?? new UserConfirmationMocker().Build()) { }

    public override Task<SignInResult> PasswordSignInAsync(string userName,
                                                           string password,
                                                           bool isPersistent,
                                                           bool lockoutOnFailure)
        => _passwordSignInAsyncCallback(userName, password, isPersistent, lockoutOnFailure);

    public FakeSignInManager SetupPasswordSignInAsync(SignInResult result)
        => SetupPasswordSignInAsync((_, _, _, _) => Task.FromResult(result));

    public FakeSignInManager SetupPasswordSignInAsync(Func<string, string, bool, bool, Task<SignInResult>> callback) {
        _passwordSignInAsyncCallback = callback;

        return this;
    }
}
