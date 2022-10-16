using ServiceStack.DataAnnotations;
using SlaytonNichols.Posts.Service.Core.Dtos;

namespace SlaytonNichols.Services;

[Description("Blog post")]
[Notes("Captures a post")]
public class PostEndpoint
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
public class QueryPosts : GetPostsRequest, IReturn<QueryResponse<PostEndpoint>>
{
}

[Tag("posts"), Description("Create a new post")]
[Route("/posts", "POST")]
[ValidateHasRole("Admin")]
public class CreatePost : CreatePostRequest, IReturn<IdResponse>
{
}

[Tag("posts"), Description("Update an existing post")]
[Route("/posts/{Id}", "PATCH")]
[ValidateHasRole("Admin")]
public class UpdatePost : UpdatePostRequest, IReturn<IdResponse>
{
}

[Tag("posts"), Description("Delete a Post")]
[Route("/posts/{Id}", "DELETE")]
[ValidateHasRole("Admin")]
public class DeletePost : DeletePostRequest, IReturnVoid
{
}