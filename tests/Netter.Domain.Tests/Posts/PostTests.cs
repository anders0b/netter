using FluentAssertions;
using Netter.Domain.Posts;

namespace Netter.Domain.Tests.Posts;

public class PostTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreatePost()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var content = "This is my first post!";

        // Act
        var post = new Post(userId, content);

        // Assert
        post.UserId.Should().Be(userId);
        post.Content.Should().Be(content);
        post.IsDeleted.Should().BeFalse();
        post.Id.Should().NotBe(Guid.Empty);
        post.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.Empty;
        var content = "This is a post";

        // Act
        var act = () => new Post(userId, content);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*User ID*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidContent_ShouldThrowArgumentException(string invalidContent)
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var act = () => new Post(userId, invalidContent);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Content*");
    }

    [Fact]
    public void Constructor_WithContentExceeding500Characters_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var longContent = new string('a', 501);

        // Act
        var act = () => new Post(userId, longContent);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot exceed 500 characters*");
    }

    [Fact]
    public void Constructor_WithContentExactly500Characters_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var content = new string('a', 500);

        // Act
        var post = new Post(userId, content);

        // Assert
        post.Content.Should().HaveLength(500);
    }

    [Fact]
    public void Constructor_ShouldTrimContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var content = "  This has whitespace  ";

        // Act
        var post = new Post(userId, content);

        // Assert
        post.Content.Should().Be("This has whitespace");
    }

    [Fact]
    public void UpdateContent_WithValidData_ShouldUpdatePostContent()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), "Original content");
        var newContent = "Updated content";
        var originalUpdatedAt = post.UpdatedAt;

        // Act
        post.UpdateContent(newContent);

        // Assert
        post.Content.Should().Be(newContent);
        post.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void UpdateContent_OnDeletedPost_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), "Original content");
        post.Delete();

        // Act
        var act = () => post.UpdateContent("New content");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*deleted post*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateContent_WithInvalidContent_ShouldThrowArgumentException(string invalidContent)
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), "Original content");

        // Act
        var act = () => post.UpdateContent(invalidContent);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Content*");
    }

    [Fact]
    public void UpdateContent_WithContentExceeding500Characters_ShouldThrowArgumentException()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), "Original content");
        var longContent = new string('a', 501);

        // Act
        var act = () => post.UpdateContent(longContent);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot exceed 500 characters*");
    }

    [Fact]
    public void Delete_ShouldSetIsDeletedToTrue()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), "Content");
        var originalUpdatedAt = post.UpdatedAt;

        // Act
        post.Delete();

        // Assert
        post.IsDeleted.Should().BeTrue();
        post.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Restore_ShouldSetIsDeletedToFalse()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), "Content");
        post.Delete();
        var originalUpdatedAt = post.UpdatedAt;

        // Act
        post.Restore();

        // Assert
        post.IsDeleted.Should().BeFalse();
        post.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }
}