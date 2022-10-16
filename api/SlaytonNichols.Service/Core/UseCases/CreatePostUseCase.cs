using SlaytonNichols.Service.Infrastructure.MongoDb.Entities;
using SlaytonNichols;
using MongoDB.Driver;
using SlaytonNichols.Service.Core.Dtos;
using SlaytonNichols.Service.Infrastructure.MongoDb.Repositories;
using MongoDB.Bson;

namespace SlaytonNichols.Service.Core.UseCases.CreatePostUseCase
{
    public class CreatePostUseCase : ICreatePostUseCase
    {
        private readonly IMongoRepository<Post> _posts;
        public CreatePostUseCase(IMongoRepository<Post> posts)
        {
            _posts = posts;
        }

        public async Task<ObjectId?> ExecuteAsync(CreatePostRequest request)
        {
            await _posts.InsertOneAsync(request);
            return _posts.AsQueryable().ToList().Where(x => x.Id == request.Id).FirstOrDefault()?.Id;
        }
    }
}