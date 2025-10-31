using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Netter.Domain.SocialInteractions;

namespace Netter.Infrastructure.Persistence.Configurations;

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.ToTable("Follows");

        builder.HasKey(f => f.Id);

        // Composite index to prevent duplicate follows and for performance
        builder.HasIndex(f => new { f.FollowerId, f.FolloweeId })
            .IsUnique();

        // Individual indexes for queries
        builder.HasIndex(f => f.FollowerId);
        builder.HasIndex(f => f.FolloweeId);
    }
}