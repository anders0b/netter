namespace Netter.Contracts.Dtos;

public class UserDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImage { get; set; }
    public bool IsActive { get; set; }
    public ICollection<string> Followers { get; private set; } = new List<string>();
    public ICollection<string> Followees { get; private set; } = new List<string>();
}