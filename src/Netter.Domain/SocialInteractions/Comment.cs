using Netter.Domain.Common;
using Netter.Domain.Posts;
using Netter.Domain.Users;

namespace Netter.Domain.SocialInteractions;

public class Comment : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid PostId { get; private set; }
    public string Content { get; private set; }
    public bool IsDeleted { get; private set; } = false;

    // Navigation properties
    public virtual User User { get; private set; } = null!;
    public virtual Post Post { get; private set; } = null!;

    private Comment() { } // EF Core constructor

    public Comment(Guid userId, Guid postId, string content)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        
        if (postId == Guid.Empty)
            throw new ArgumentException("Post ID cannot be empty", nameof(postId));
        
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));
        
        if (content.Length > 200) // Shorter than posts
            throw new ArgumentException("Comment cannot exceed 200 characters", nameof(content));

        UserId = userId;
        PostId = postId;
        Content = content.Trim();
    }

    public void UpdateContent(string content)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update a deleted comment");
        
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));
        
        if (content.Length > 200)
            throw new ArgumentException("Comment cannot exceed 200 characters", nameof(content));

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