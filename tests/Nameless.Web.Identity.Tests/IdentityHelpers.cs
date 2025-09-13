using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nameless.Testing.Tools.Mockers.AspNetCore.Http;
using Nameless.Testing.Tools.Mockers.DependencyInjection;
using Nameless.Web.Identity.Entities;
using Nameless.Web.Identity.Mockers;

namespace Nameless.Web.Identity;

public static class IdentityHelpers {
    public static readonly string JohnDoePassHash = "QWluJ3QgeW91IGEgY3VyaW91cyBvbmU/Pw==";

    public static UserManager<User> CreateUserManager(
        IUserStore<User> store = null,
        IOptions<IdentityOptions> options = null,
        IPasswordHasher<User> passwordHasher = null,
        IEnumerable<IUserValidator<User>> userValidators = null,
        IEnumerable<IPasswordValidator<User>> passwordValidators = null,
        ILookupNormalizer keyNormalizer = null,
        IdentityErrorDescriber errors = null,
        IServiceProvider services = null,
        ILogger<UserManager<User>> logger = null) {
        store ??= new UserStoreMocker().Build();
        options ??= OptionsFactory.Create(new IdentityOptions());
        passwordHasher ??= new PasswordHasherMocker().Build();
        userValidators ??= [];
        passwordValidators ??= [];
        keyNormalizer ??= new LookupNormalizerMocker().Build();
        errors ??= new IdentityErrorDescriber();
        services ??= new ServiceProviderMocker().Build();
        logger ??= NullLogger<UserManager<User>>.Instance;

        return new UserManager<User>(store, options, passwordHasher, userValidators, passwordValidators, keyNormalizer,
            errors, services, logger);
    }

    public static SignInManager<User> CreateSignInManager(
        UserManager<User> userManager = null,
        IHttpContextAccessor contextAccessor = null,
        IUserClaimsPrincipalFactory<User> claimsFactory = null,
        IOptions<IdentityOptions> optionsAccessor = null,
        ILogger<SignInManager<User>> logger = null,
        IAuthenticationSchemeProvider schemes = null,
        IUserConfirmation<User> confirmation = null) {
        userManager ??= CreateUserManager();
        contextAccessor ??= new HttpContextAccessorMocker().Build();
        optionsAccessor ??= OptionsFactory.Create(new IdentityOptions());
        claimsFactory ??= new UserClaimsPrincipalFactory<User>(userManager, optionsAccessor);
        logger ??= NullLogger<SignInManager<User>>.Instance;

        var authenticationOptions = OptionsFactory.Create(new AuthenticationOptions {
            DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme,
            DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme
        });
        schemes ??= new AuthenticationSchemeProvider(authenticationOptions);

        confirmation ??= new UserConfirmationMocker().Build();

        return new SignInManager<User>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes,
            confirmation);
    }

    public static IOptions<TOptions> CreateOptions<TOptions>(Action<TOptions> configure = null)
        where TOptions : class, new() {
        var innerConfigure = configure ?? (_ => { });
        var options = new TOptions();

        innerConfigure(options);

        return OptionsFactory.Create(options);
    }

    public static ILogger<TCategoryName> CreateLogger<TCategoryName>() {
        return NullLogger<TCategoryName>.Instance;
    }

    public static User CreateJohnDoeUser() {
        return new User {
            Id = Guid.Parse(input: "a39ca82e-c1e0-4b55-8aac-a0497265a178"),
            UserName = "JohnDoe",
            NormalizedUserName = "JOHNDOE",
            Email = "johndoe@nobody.com",
            NormalizedEmail = "JOHNDOE@NOBODY.COM",
            EmailConfirmed = true,
            PasswordHash = JohnDoePassHash,
            SecurityStamp = null,
            ConcurrencyStamp = null,
            PhoneNumber = "+351 123 456 789",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = true,
            AccessFailedCount = 0,
            FirstName = "John",
            LastName = "Doe",
            AvatarUrl = null
        };
    }
}