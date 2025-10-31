using Netter.Domain.Common;
using Netter.Domain.Posts;
using Netter.Domain.Users;

namespace Netter.Domain.SocialInteractions;

public class Like : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid PostId { get; private set; }

    // Navigation properties
    public virtual User User { get; private set; } = null!;
    public virtual Post Post { get; private set; } = null!;

    private Like() { } // EF Core constructor

    public Like(Guid userId, Guid postId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        
        if (postId == Guid.Empty)
            throw new ArgumentException("Post ID cannot be empty", nameof(postId));

        UserId = userId;
        PostId = postId;
    }
}