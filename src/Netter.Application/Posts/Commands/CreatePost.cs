using MediatR;
using Netter.Application.Common;

namespace Netter.Application.Posts.Commands;

public record CreatePostCommand(
    Guid UserId,
    string Content) : IRequest<CreatePostResult>;

public record CreatePostResult(
    Guid PostId,
    Guid UserId,
    string Content,
    DateTime CreatedAt);

public class CreatePostHandler : IRequestHandler<CreatePostCommand, CreatePostResult>
{
    private readonly IRepository<Netter.Domain.Posts.Post> _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePostHandler(
        IRepository<Netter.Domain.Posts.Post> postRepository, 
        IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreatePostResult> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = new Netter.Domain.Posts.Post(
            request.UserId,
            request.Content);

        await _postRepository.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreatePostResult(
            post.Id,
            post.UserId,
            post.Content,
            post.CreatedAt);
    }
}