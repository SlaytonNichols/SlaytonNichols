using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Domain.Models;
using Entity = SlaytonNichols.Posts.Service.Infrastructure.Data.Documents.Post;

namespace SlaytonNichols.Posts.Service.Core.Mappers;

public interface IPostMapper
{
    Task<IEnumerable<Post>> Map(IEnumerable<Entity> posts);
    Task<Post> Map(Entity post);
    Task<Entity> MapCreatePostRequest(CreatePostRequest post);
    Task<Entity> MapUpdatePostRequest(UpdatePostRequest request);
}