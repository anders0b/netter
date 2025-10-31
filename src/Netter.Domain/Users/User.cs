using Netter.Domain.Common;
using Netter.Domain.SocialInteractions;

namespace Netter.Domain.Users;

public class User : BaseEntity
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string DisplayName { get; private set; }
    public string? Bio { get; private set; }
    public string? ProfileImageUrl { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual ICollection<Follow> Followers { get; private set; } = new List<Follow>();
    public virtual ICollection<Follow> Following { get; private set; } = new List<Follow>();

    private User() { } // EF Core constructor

    public User(string username, string email, string displayName)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty", nameof(displayName));

        Username = username.ToLowerInvariant();
        Email = email.ToLowerInvariant();
        DisplayName = displayName;
    }

    public void UpdateProfile(string displayName, string? bio, string? profileImageUrl)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty", nameof(displayName));

        DisplayName = displayName;
        Bio = bio;
        ProfileImageUrl = profileImageUrl;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }

    public void Activate()
    {
        IsActive = true;
        UpdateTimestamp();
    }
}