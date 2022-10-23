using MongoDB.Bson;
using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Domain.Models;
using Entity = SlaytonNichols.Posts.Service.Infrastructure.Data.Documents.Post;

namespace SlaytonNichols.Posts.Service.Core.Mappers;

public class PostMapper : IPostMapper
{
    public IEnumerable<Post> Map(IEnumerable<Entity> posts)
    {
        var response = new List<Post>();
        foreach (var post in posts)
            response.Add(Map(post));
        return response;
    }
    public Post Map(Entity post) => new Post
    {
        Id = post.Id.ToString(),
        MdText = post.MdText,
        Title = post.Title,
        Path = post.Path,
        Summary = post.Summary,
        Draft = post.Draft
    };

    public Entity MapCreatePostRequest(CreatePostRequest request) => new Entity
    {
        MdText = request.MdText,
        Title = request.Title,
        Path = request.Path,
        Summary = request.Summary,
        Draft = request.Draft
    };

    public Entity MapUpdatePostRequest(UpdatePostRequest request) => new Entity
    {
        //TODO: This is a hack. Need to figure out how to map the Id to the ObjectId... Github Copilot wrote that for me.
        Id = new ObjectId(request.Id),
        MdText = request.MdText,
        Title = request.Title,
        Path = request.Path,
        Summary = request.Summary,
        Draft = request.Draft
    };
}