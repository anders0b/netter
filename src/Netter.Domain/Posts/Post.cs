using Netter.Domain.Common;
using Netter.Domain.Users;

namespace Netter.Domain.Posts;

public class Post : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Content { get; private set; }
    public bool IsDeleted { get; private set; } = false;

    // Navigation properties
    public virtual User User { get; private set; } = null!;

    private Post() { } // EF Core constructor

    public Post(Guid userId, string content)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));
        
        if (content.Length > 500) // Simple business rule
            throw new ArgumentException("Content cannot exceed 500 characters", nameof(content));

        UserId = userId;
        Content = content.Trim();
    }

    public void UpdateContent(string content)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update a deleted post");
        
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));
        
        if (content.Length > 500)
            throw new ArgumentException("Content cannot exceed 500 characters", nameof(content));

        Content = content.Trim();
        UpdateTimestamp();
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdateTimestamp();
    }

    public void Restore()
    {
        IsDeleted = false;
        UpdateTimestamp();
    }
}