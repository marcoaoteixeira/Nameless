using Moq;
using Nameless.Testing.Tools.Mockers;
using Nameless.Web.Identity.Domains.UserRefreshTokens;
using Nameless.Web.Identity.Entities;

namespace Nameless.Web.Identity.Mockers;

public class UserRefreshTokenManagerMocker : Mocker<IUserRefreshTokenManager> {
    public UserRefreshTokenManagerMocker WithCreateAsync(UserRefreshToken returnValue) {
        MockInstance
            .Setup(mock => mock.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(returnValue));

        return this;
    }

    public UserRefreshTokenManagerMocker WithCleanUpAsync(int returnValue) {
        MockInstance
            .Setup(mock => mock.CleanUpAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(returnValue));

        return this;
    }

    public UserRefreshTokenManagerMocker WithRevokeAsync(int returnValue) {
        MockInstance
            .Setup(mock => mock.RevokeAsync(It.IsAny<User>(), It.IsAny<RevokeUserRefreshTokenMetadata>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(returnValue));

        return this;
    }
}