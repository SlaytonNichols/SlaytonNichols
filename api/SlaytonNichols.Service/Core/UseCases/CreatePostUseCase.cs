using SlaytonNichols.Service.Infrastructure.MongoDb.Entities;
using SlaytonNichols;
using MongoDB.Driver;
using SlaytonNichols.Service.Core.Dtos;
using SlaytonNichols.Service.Infrastructure.MongoDb.Repositories;

namespace SlaytonNichols.Service.Core.UseCases.CreatePostUseCase
{
    public class CreatePostUseCase : ICreatePostUseCase
    {
        private readonly IMongoRepository<Post> _posts;
        public CreatePostUseCase(IMongoRepository<Post> posts)
        {
            _posts = posts;
        }

        public async Task ExecuteAsync(CreatePostRequest request)
        {
            await _posts.InsertOneAsync(request);
        }
    }
}