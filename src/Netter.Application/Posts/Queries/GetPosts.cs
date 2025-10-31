using MediatR;
using Netter.Application.Common;

namespace Netter.Application.Posts.Queries;

public record GetPostsQuery() : IRequest<IEnumerable<GetPostsResult>>;

public record GetPostsResult(
    Guid PostId,
    Guid UserId,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public class GetPostsHandler : IRequestHandler<GetPostsQuery, IEnumerable<GetPostsResult>>
{
    private readonly IRepository<Netter.Domain.Posts.Post> _postRepository;

    public GetPostsHandler(IRepository<Netter.Domain.Posts.Post> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<IEnumerable<GetPostsResult>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await _postRepository.GetAllAsync(cancellationToken);
        
        return posts
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Select(post => new GetPostsResult(
                post.Id,
                post.UserId,
                post.Content,
                post.CreatedAt,
                post.UpdatedAt));
    }
}