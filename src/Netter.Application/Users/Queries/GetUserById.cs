using MediatR;
using Netter.Application.Common;

namespace Netter.Application.Users.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<GetUserByIdResult?>;

public record GetUserByIdResult(
    Guid UserId,
    string Username,
    string Email,
    string DisplayName,
    string? Bio,
    string? ProfileImageUrl,
    bool IsActive,
    DateTime CreatedAt);

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdResult?>
{
    private readonly IRepository<Netter.Domain.Users.User> _userRepository;

    public GetUserByIdHandler(IRepository<Netter.Domain.Users.User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetUserByIdResult?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
            return null;

        return new GetUserByIdResult(
            user.Id,
            user.Username,
            user.Email,
            user.DisplayName,
            user.Bio,
            user.ProfileImageUrl,
            user.IsActive,
            user.CreatedAt);
    }
}