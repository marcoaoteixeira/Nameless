using System.Data;
using Moq;

namespace Nameless.Data;

public class DbConnectionExtensionsTests {
    // --- EnsureOpen ---

    [Fact]
    public void EnsureOpen_WhenConnectionClosed_OpensConnection() {
        // arrange
        var mock = new Mock<IDbConnection>();
        mock.Setup(c => c.State).Returns(ConnectionState.Closed);

        // act
        mock.Object.EnsureOpen();

        // assert
        mock.Verify(c => c.Open(), Times.Once);
    }

    [Fact]
    public void EnsureOpen_WhenConnectionAlreadyOpen_DoesNotCallOpen() {
        // arrange
        var mock = new Mock<IDbConnection>();
        mock.Setup(c => c.State).Returns(ConnectionState.Open);

        // act
        mock.Object.EnsureOpen();

        // assert
        mock.Verify(c => c.Open(), Times.Never);
    }
}
