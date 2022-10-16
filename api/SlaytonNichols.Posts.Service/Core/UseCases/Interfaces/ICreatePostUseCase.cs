using SlaytonNichols;
using SlaytonNichols.Posts.Service.Core.Dtos;
using MongoDB.Bson;

namespace SlaytonNichols.Posts.Service.Core.UseCases.CreatePostUseCase
{
    public interface ICreatePostUseCase
    {
        Task<ObjectId?> ExecuteAsync(CreatePostRequest request);
    }
}