using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Moq;
using Nameless.Testing.Tools.Mockers;
using Nameless.Web.Identity.Entities;

namespace Nameless.Web.Identity.Mockers;

public class UserStoreMocker : Mocker<UserStoreBase<User, Role, Guid, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>> {
    public UserStoreMocker()
        : base(args: [new IdentityErrorDescriber()]) { }

    public UserStoreMocker WithFindByIdAsync(User returnValue) {
        MockInstance
            .Setup(mock => mock.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(returnValue));

        return this;
    }

    public UserStoreMocker WithGetUserIdAsync(string returnValue) {
        MockInstance
            .Setup(mock => mock.GetUserIdAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(returnValue));

        return this;
    }

    public UserStoreMocker WithGetUserNameAsync(string returnValue) {
        MockInstance
            .Setup(mock => mock.GetUserNameAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(returnValue));

        return this;
    }

    public UserStoreMocker WithGetEmailAsync(string returnValue) {
        MockInstance
            .Setup(mock => mock.GetEmailAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(returnValue));

        return this;
    }

    public UserStoreMocker WithGetSecurityStampAsync(string returnValue) {
        MockInstance
            .Setup(mock => mock.GetSecurityStampAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(returnValue));

        return this;
    }

    public UserStoreMocker WithGetClaimsAsync(IList<Claim> returnValue) {
        MockInstance
            .Setup(mock => mock.GetClaimsAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(returnValue));

        return this;
    }
}