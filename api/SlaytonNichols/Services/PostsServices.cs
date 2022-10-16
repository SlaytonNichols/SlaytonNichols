using SlaytonNichols.Posts.Service.Core.UseCases.CreatePostUseCase;
using SlaytonNichols.Posts.Service.Core.UseCases.DeletePostUseCase;
using SlaytonNichols.Posts.Service.Core.UseCases.GetPostsUseCase;
using SlaytonNichols.Posts.Service.Core.UseCases.UpdatePostUseCase;

namespace SlaytonNichols.Services;

public class PostsServices : ServiceStack.Service
{
    private readonly ILogger<PostsServices> _logger;
    private readonly IGetPostsUseCase _getPostsUseCase;
    private readonly ICreatePostUseCase _createPostUseCase;
    private readonly IUpdatePostUseCase _updatePostUseCase;
    private readonly IDeletePostUseCase _deletePostUseCase;

    public PostsServices(ILogger<PostsServices> logger,
                        IGetPostsUseCase getPostsUseCase,
                        ICreatePostUseCase createPostUseCase,
                        IUpdatePostUseCase updatePostUseCase,
                        IDeletePostUseCase deletePostUseCase)
    {
        _logger = logger;
        _getPostsUseCase = getPostsUseCase;
        _createPostUseCase = createPostUseCase;
        _updatePostUseCase = updatePostUseCase;
        _deletePostUseCase = deletePostUseCase;
    }


    public async Task<object> Get(QueryPosts query)
    {
        var posts = await _getPostsUseCase.ExecuteAsync(query);
        return posts;
    }

    public async Task<object> Post(CreatePost query)
    {
        var id = await _createPostUseCase.ExecuteAsync(query);
        return new IdResponse { Id = id.ToString() };
    }

    public async Task<object> Patch(UpdatePost query)
    {
        await _updatePostUseCase.ExecuteAsync(query);

        return new IdResponse { Id = query.Id.ToString() };
    }

    public async Task Delete(DeletePost query)
    {
        await _deletePostUseCase.ExecuteAsync(query);
    }
}