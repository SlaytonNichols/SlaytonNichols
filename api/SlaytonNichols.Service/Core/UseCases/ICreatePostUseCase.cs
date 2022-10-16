using SlaytonNichols.Service.Infrastructure.MongoDb.Entities;
using SlaytonNichols;
using SlaytonNichols.Service.Core.Dtos;
using MongoDB.Bson;

namespace SlaytonNichols.Service.Core.UseCases.CreatePostUseCase
{
    public interface ICreatePostUseCase
    {
        Task<ObjectId?> ExecuteAsync(CreatePostRequest request);
    }
}