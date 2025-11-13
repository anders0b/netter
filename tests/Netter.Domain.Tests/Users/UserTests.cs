using FluentAssertions;
using Netter.Domain.Users;

namespace Netter.Domain.Tests.Users;

public class UserTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var username = "johndoe";
        var email = "john@example.com";
        var displayName = "John Doe";

        // Act
        var user = new User(username, email, displayName);

        // Assert
        user.Username.Should().Be(username.ToLowerInvariant());
        user.Email.Should().Be(email.ToLowerInvariant());
        user.DisplayName.Should().Be(displayName);
        user.IsActive.Should().BeTrue();
        user.Bio.Should().BeNull();
        user.ProfileImageUrl.Should().BeNull();
        user.Id.Should().NotBe(Guid.Empty);
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidUsername_ShouldThrowArgumentException(string invalidUsername)
    {
        // Arrange
        var email = "john@example.com";
        var displayName = "John Doe";

        // Act
        var act = () => new User(invalidUsername, email, displayName);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Username*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidEmail_ShouldThrowArgumentException(string invalidEmail)
    {
        // Arrange
        var username = "johndoe";
        var displayName = "John Doe";

        // Act
        var act = () => new User(username, invalidEmail, displayName);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Email*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidDisplayName_ShouldThrowArgumentException(string invalidDisplayName)
    {
        // Arrange
        var username = "johndoe";
        var email = "john@example.com";

        // Act
        var act = () => new User(username, email, invalidDisplayName);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Display name*");
    }

    [Fact]
    public void UpdateProfile_WithValidData_ShouldUpdateUserProfile()
    {
        // Arrange
        var user = new User("johndoe", "john@example.com", "John Doe");
        var newDisplayName = "Johnny Doe";
        var newBio = "Software developer";
        var newProfileImageUrl = "https://example.com/avatar.jpg";
        var originalCreatedAt = user.CreatedAt;
        
        // Act
        user.UpdateProfile(newDisplayName, newBio, newProfileImageUrl);

        // Assert
        user.DisplayName.Should().Be(newDisplayName);
        user.Bio.Should().Be(newBio);
        user.ProfileImageUrl.Should().Be(newProfileImageUrl);
        user.UpdatedAt.Should().BeAfter(originalCreatedAt);
        user.Username.Should().Be("johndoe"); // Should not change
        user.Email.Should().Be("john@example.com"); // Should not change
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateProfile_WithInvalidDisplayName_ShouldThrowArgumentException(string invalidDisplayName)
    {
        // Arrange
        var user = new User("johndoe", "john@example.com", "John Doe");

        // Act
        var act = () => user.UpdateProfile(invalidDisplayName, "Bio", null);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Display name*");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var user = new User("johndoe", "john@example.com", "John Doe");
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.Deactivate();

        // Assert
        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var user = new User("johndoe", "john@example.com", "John Doe");
        user.Deactivate();
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.Activate();

        // Assert
        user.IsActive.Should().BeTrue();
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Equals_WithSameId_ShouldReturnTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user1 = new User("user1", "user1@example.com", "User 1");
        var user2 = new User("user2", "user2@example.com", "User 2");
        
        // Use reflection to set the same ID (for testing purposes)
        typeof(User).GetProperty("Id")!.SetValue(user1, id);
        typeof(User).GetProperty("Id")!.SetValue(user2, id);

        // Act & Assert
        user1.Should().Be(user2);
        (user1 == user2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentId_ShouldReturnFalse()
    {
        // Arrange
        var user1 = new User("user1", "user1@example.com", "User 1");
        var user2 = new User("user2", "user2@example.com", "User 2");

        // Act & Assert
        user1.Should().NotBe(user2);
        (user1 != user2).Should().BeTrue();
    }
}