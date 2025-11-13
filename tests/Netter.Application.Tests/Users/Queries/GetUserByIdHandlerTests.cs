using FluentAssertions;
using Netter.Application.Common;
using Netter.Application.Users.Queries;
using Netter.Domain.Users;
using NSubstitute;

namespace Netter.Application.Tests.Users.Queries;

public class GetUserByIdHandlerTests
{
    private readonly IRepository<User> _userRepository;
    private readonly GetUserByIdHandler _handler;

    public GetUserByIdHandlerTests()
    {
        _userRepository = Substitute.For<IRepository<User>>();
        _handler = new GetUserByIdHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ShouldReturnUserResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("johndoe", "john@example.com", "John Doe");
        
        // Set the ID using reflection (since it's protected)
        typeof(User).GetProperty("Id")!.SetValue(user, userId);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        var query = new GetUserByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.Username.Should().Be("johndoe");
        result.Email.Should().Be("john@example.com");
        result.DisplayName.Should().Be("John Doe");
        result.IsActive.Should().BeTrue();
        result.Bio.Should().BeNull();
        result.ProfileImageUrl.Should().BeNull();

        // Verify repository was called
        await _userRepository.Received(1).GetByIdAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        var query = new GetUserByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        // Verify repository was called
        await _userRepository.Received(1).GetByIdAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldIncludeUserProfileData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("johndoe", "john@example.com", "John Doe");
        user.UpdateProfile("John Updated", "Software Developer", "https://example.com/avatar.jpg");
        
        typeof(User).GetProperty("Id")!.SetValue(user, userId);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        var query = new GetUserByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.DisplayName.Should().Be("John Updated");
        result.Bio.Should().Be("Software Developer");
        result.ProfileImageUrl.Should().Be("https://example.com/avatar.jpg");
    }

    [Fact]
    public async Task Handle_WithDeactivatedUser_ShouldReturnUserWithIsActiveFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("johndoe", "john@example.com", "John Doe");
        user.Deactivate();
        
        typeof(User).GetProperty("Id")!.SetValue(user, userId);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        var query = new GetUserByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.IsActive.Should().BeFalse();
    }
}