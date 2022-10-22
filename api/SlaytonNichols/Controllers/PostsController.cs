using ServiceStack.DataAnnotations;
using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Core.UseCases.CreatePostUseCase;
using SlaytonNichols.Posts.Service.Core.UseCases.DeletePostUseCase;
using SlaytonNichols.Posts.Service.Core.UseCases.GetPostsUseCase;
using SlaytonNichols.Posts.Service.Core.UseCases.UpdatePostUseCase;

namespace SlaytonNichols.Controllers;
[Tag("posts"), Description("Find posts")]
[Route("/posts", "GET")]
[Route("/posts/{Path}", "GET")]
public class GetPosts : GetPostsRequest, IReturn<GetPostsResponse> { }

[Tag("posts"), Description("Create a new post")]
[Route("/posts", "POST")]
[ValidateHasRole("Admin")]
public class PostPost : CreatePostRequest, IReturnVoid { }
[Tag("posts"), Description("Update an existing post")]
[Route("/posts/{Id}", "PATCH")]
[ValidateHasRole("Admin")]
public class PatchPost : UpdatePostRequest, IReturnVoid { }

[Tag("posts"), Description("Delete a Post")]
[Route("/posts/{Id}", "DELETE")]
[ValidateHasRole("Admin")]
public class DeletePost : DeletePostRequest, IReturnVoid { }
public class PostsController : ServiceStack.Service
{
    private readonly ILogger<PostsController> _logger;
    private readonly IGetPostsUseCase _getPostsUseCase;
    private readonly ICreatePostUseCase _createPostUseCase;
    private readonly IUpdatePostUseCase _updatePostUseCase;
    private readonly IDeletePostUseCase _deletePostUseCase;

    public PostsController(ILogger<PostsController> logger,
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
    public async Task<object> Get(GetPosts query)
    {
        var posts = await _getPostsUseCase.ExecuteAsync(query);
        return posts;
    }
    public async Task Post(PostPost query)
    {
        await _createPostUseCase.ExecuteAsync(query);
    }
    public async Task Patch(PatchPost query)
    {
        await _updatePostUseCase.ExecuteAsync(query);
    }

    public async Task Delete(DeletePost query)
    {
        await _deletePostUseCase.ExecuteAsync(query);
    }
}