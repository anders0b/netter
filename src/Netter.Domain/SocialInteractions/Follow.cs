using Netter.Domain.Common;
using Netter.Domain.Users;

namespace Netter.Domain.SocialInteractions;

public class Follow : BaseEntity
{
    public Guid FollowerId { get; private set; }
    public Guid FolloweeId { get; private set; }

    // Navigation properties
    public virtual User Follower { get; private set; } = null!;
    public virtual User Followee { get; private set; } = null!;

    private Follow() { } // EF Core constructor

    public Follow(Guid followerId, Guid followeeId)
    {
        if (followerId == Guid.Empty)
            throw new ArgumentException("Follower ID cannot be empty", nameof(followerId));
        
        if (followeeId == Guid.Empty)
            throw new ArgumentException("Followee ID cannot be empty", nameof(followeeId));
        
        if (followerId == followeeId)
            throw new ArgumentException("User cannot follow themselves");

        FollowerId = followerId;
        FolloweeId = followeeId;
    }
}