using Moq;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Common.Testing.Tools.Mockers.Examples;

[UnitTest]
public class CallSequenceExample
{
    [Fact]
    public void WhenUseSequence_WhenExecuteInSequence_ThenMockWorksAsExpected()
    {
        // arrange
        var sequence = new MockSequence();
        var repository = new RepositoryMocker(sequence)
            .WithRead(returnValue: null!)
            .WithCreate(returnValue: true)
            .Build();

        var sut = new UserService(repository);

        // act
        // UpSert action calls first Read
        // if not null is returned, them it calls Update;
        // otherwise, Insert
        var actual = sut.UpSert(1, new object());

        // assert
        Assert.True(actual);
    }

    [Fact]
    public void WhenUseSequence_WhenExecuteOutOfSequence_ThenMockThrowsException()
    {
        // arrange
        var sequence = new MockSequence();
        var repository = new RepositoryMocker(sequence)
            .WithRead(returnValue: new object()) // if read returns
            .WithCreate(returnValue: true) // next should be Update, not Create.
            .WithUpdate(returnValue: true) // this will never happen.
            .Build();
        var sut = new UserService(repository);

        // act
        // UpSert action calls first Read
        // if not null is returned, them it calls Update;
        // otherwise, Insert
        var record = Record.Exception(() => sut.UpSert(1, new object()));

        // assert
        Assert.IsType<MockException>(record);
    }
}

public class RepositoryMocker : Mocker<IRepository>
{
    public RepositoryMocker(MockSequence sequence)
        : base (sequence) { }

    public RepositoryMocker WithExists(bool returnValue)
    {
        MockInstance
            .Setup(mock => mock.Exists(It.IsAny<int>()))
            .Returns(returnValue);

        return this;
    }

    public RepositoryMocker WithCreate(bool returnValue)
    {
        MockInstance
            .Setup(mock => mock.Create(It.IsAny<object>()))
            .Returns(returnValue);

        return this;
    }

    public RepositoryMocker WithRead(object returnValue)
    {
        MockInstance
            .Setup(mock => mock.Read(It.IsAny<int>()))
            .Returns(returnValue);

        return this;
    }

    public RepositoryMocker WithUpdate(bool returnValue)
    {
        MockInstance
            .Setup(mock => mock.Update(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(returnValue);

        return this;
    }

    public RepositoryMocker WithDelete(bool returnValue)
    {
        MockInstance
            .Setup(mock => mock.Delete(It.IsAny<int>()))
            .Returns(returnValue);

        return this;
    }
}

public interface IRepository
{
    bool Exists(int id);
    bool Create(object entity);
    object Read(int id);
    bool Update(int id, object entity);
    bool Delete(int id);
}

public class UserService
{
    private readonly IRepository _repository;

    public UserService(IRepository repository)
    {
        _repository = repository;
    }

    public bool UpSert(int id, object user)
    {
        var current = _repository.Read(id);

        return current is null
            ? _repository.Create(user)
            : _repository.Update(id, user);
    }
}