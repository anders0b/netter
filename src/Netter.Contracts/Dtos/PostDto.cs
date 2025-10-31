namespace Netter.Contracts.Dtos;

public class PostDto
{
    public string? UserId { get; set; }
    public string? Content { get; set; }
    public bool? IsDeleted { get; set; }
}