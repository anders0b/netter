using FluentAssertions;
using Netter.Application.Common;
using Netter.Application.Users.Commands;
using Netter.Domain.Users;
using NSubstitute;

namespace Netter.Application.Tests.Users.Commands;

public class CreateUserHandlerTests
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerTests()
    {
        _userRepository = Substitute.For<IRepository<User>>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CreateUserHandler(_userRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateUserAndReturnResult()
    {
        // Arrange
        var command = new CreateUserCommand(
            Username: "johndoe",
            Email: "john@example.com",
            DisplayName: "John Doe"
        );

        _userRepository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<User>()));

        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be("johndoe");
        result.Email.Should().Be("john@example.com");
        result.DisplayName.Should().Be("John Doe");
        result.UserId.Should().NotBe(Guid.Empty);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));

        // Verify interactions
        await _userRepository.Received(1).AddAsync(
            Arg.Is<User>(u => 
                u.Username == "johndoe" && 
                u.Email == "john@example.com" &&
                u.DisplayName == "John Doe"),
            Arg.Any<CancellationToken>());

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("", "john@example.com", "John Doe")]
    [InlineData("johndoe", "", "John Doe")]
    [InlineData("johndoe", "john@example.com", "")]
    public async Task Handle_WithInvalidData_ShouldThrowArgumentException(
        string username, 
        string email, 
        string displayName)
    {
        // Arrange
        var command = new CreateUserCommand(username, email, displayName);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();

        // Verify no interactions with dependencies
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldNormalizeUsernameAndEmailToLowercase()
    {
        // Arrange
        var command = new CreateUserCommand(
            Username: "JohnDoe",
            Email: "John@Example.COM",
            DisplayName: "John Doe"
        );

        _userRepository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<User>()));

        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Username.Should().Be("johndoe");
        result.Email.Should().Be("john@example.com");
        result.DisplayName.Should().Be("John Doe"); // DisplayName should NOT be lowercased
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationTokenToRepository()
    {
        // Arrange
        var command = new CreateUserCommand("johndoe", "john@example.com", "John Doe");
        var cancellationToken = new CancellationToken();

        _userRepository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<User>()));

        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(1));

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert - Verify the cancellation token was passed through
        await _userRepository.Received(1).AddAsync(Arg.Any<User>(), cancellationToken);
        await _unitOfWork.Received(1).SaveChangesAsync(cancellationToken);
    }
}