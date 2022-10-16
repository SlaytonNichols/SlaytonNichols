using MongoDB.Bson;
using ServiceStack.DataAnnotations;
using SlaytonNichols.Service.Core.Dtos;
using SlaytonNichols.Service.Core.UseCases.CreatePostUseCase;
using SlaytonNichols.Service.Core.UseCases.DeletePostUseCase;
using SlaytonNichols.Service.Core.UseCases.GetPostsUseCase;
using SlaytonNichols.Service.Core.UseCases.UpdatePostUseCase;

namespace SlaytonNichols;

[Description("Blog post")]
[Notes("Captures a post")]
public class Post : AuditBase
{
    public string Id { get; set; }
    public string MdText { get; set; }
    public string Title { get; set; }
    public string Path { get; set; }
    public string Summary { get; set; }
    public bool Draft { get; set; }
}

[Tag("posts"), Description("Find posts")]
[Route("/posts", "GET")]
[Route("/posts/{Path}", "GET")]
[AutoApply(Behavior.AuditQuery)]
public class QueryPosts : IReturn<IEnumerable<Post>>
{
    public string? Path { get; set; }
}

[Tag("posts"), Description("Create a new post")]
[Route("/posts", "POST")]
[ValidateHasRole("Admin")]
[AutoApply(Behavior.AuditCreate)]
public class CreatePost : IReturn<IdResponse>
{
    public string MdText { get; set; }
    public string Title { get; set; }
    public string Path { get; set; }
    public string Summary { get; set; }
    public bool Draft { get; set; }
}

[Tag("posts"), Description("Update an existing post")]
[Route("/posts/{Id}", "PATCH")]
[ValidateHasRole("Admin")]
[AutoApply(Behavior.AuditModify)]
public class UpdatePost : IReturn<IdResponse>
{
    public string Id { get; set; }
    public string MdText { get; set; }
    public string Title { get; set; }
    public string Path { get; set; }
    public string Summary { get; set; }
    public bool Draft { get; set; }
}

[Tag("posts"), Description("Delete a Post")]
[Route("/posts/{Id}", "DELETE")]
[ValidateHasRole("Admin")]
[AutoApply(Behavior.AuditSoftDelete)]
public class DeletePost : IReturnVoid
{
    public string Id { get; set; }
}

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
        var request = new GetPostsRequest
        {
            Path = query.Path
        };
        var posts = await _getPostsUseCase.ExecuteAsync(request);
        return posts;
    }

    public async Task<object> Post(CreatePost query)
    {
        var request = new CreatePostRequest
        {
            Draft = query.Draft,
            MdText = query.MdText,
            Path = query.Path,
            Summary = query.Summary,
            Title = query.Title
        };

        var id = await _createPostUseCase.ExecuteAsync(request);
        return new IdResponse { Id = id.ToString() };
    }

    public async Task<object> Patch(UpdatePost query)
    {
        var request = new UpdatePostRequest
        {
            Path = query.Path,
            Summary = query.Summary,
            Title = query.Title,
            MdText = query.MdText,
            Draft = query.Draft,
            Id = new ObjectId(query.Id)
        };
        await _updatePostUseCase.ExecuteAsync(request);

        return new HttpResult(request, System.Net.HttpStatusCode.OK);
    }

    public async Task<object> Delete(DeletePost query)
    {
        var request = new DeletePostRequest
        {
            Id = new ObjectId(query.Id)
        };
        await _deletePostUseCase.ExecuteAsync(request);

        return new HttpResult(request, System.Net.HttpStatusCode.OK);
    }
}