using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Domain.Models;
using Entity = SlaytonNichols.Posts.Service.Infrastructure.Data.Documents.Post;

namespace SlaytonNichols.Posts.Service.Core.Mappers;

public interface IPostMapper
{
    IEnumerable<Post> Map(IEnumerable<Entity> posts);
    Post Map(Entity post);
    Entity MapCreatePostRequest(CreatePostRequest post);
    Entity MapUpdatePostRequest(UpdatePostRequest request);
}