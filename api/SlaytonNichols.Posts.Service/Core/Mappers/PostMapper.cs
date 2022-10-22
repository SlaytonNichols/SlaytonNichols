using MongoDB.Bson;
using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Domain.Models;
using Entity = SlaytonNichols.Posts.Service.Infrastructure.Data.Documents.Post;

namespace SlaytonNichols.Posts.Service.Core.Mappers;

public class PostMapper : IPostMapper
{
    public async Task<IEnumerable<Post>> Map(IEnumerable<Entity> posts)
    {
        var response = new List<Post>();
        foreach (var post in posts)
            response.Add(await Map(post));
        return response;
    }
    public async Task<Post> Map(Entity post)
    {
        return new Post
        {
            Id = post.Id.ToString(),
            MdText = post.MdText,
            Title = post.Title,
            Path = post.Path,
            Summary = post.Summary,
            Draft = post.Draft
        };
    }

    public async Task<Entity> MapCreatePostRequest(CreatePostRequest request)
    {
        return new Entity
        {
            MdText = request.MdText,
            Title = request.Title,
            Path = request.Path,
            Summary = request.Summary,
            Draft = request.Draft
        };
    }

    public async Task<Entity> MapUpdatePostRequest(UpdatePostRequest request)
    {
        return new Entity
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
}