using MediatR;
using Netter.Application.Common;
using Netter.Domain.Users;

namespace Netter.Application.Users.Commands;

public record CreateUserCommand(
    string Username,
    string Email,
    string DisplayName) : IRequest<CreateUserResult>;

public record CreateUserResult(
    Guid UserId,
    string Username,
    string Email,
    string DisplayName,
    DateTime CreatedAt);

public class CreateUserHandler(
    IRepository<User> userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateUserCommand, CreateUserResult>
{
    public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User(
            request.Username,
            request.Email,
            request.DisplayName);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateUserResult(
            user.Id,
            user.Username,
            user.Email,
            user.DisplayName,
            user.CreatedAt);
    }
}