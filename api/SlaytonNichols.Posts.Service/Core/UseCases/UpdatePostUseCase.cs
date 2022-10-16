using SlaytonNichols;
using MongoDB.Driver;
using SlaytonNichols.Posts.Service.Core.Dtos;
using MongoDB.Bson;
using SlaytonNichols.Posts.Service.Infrastructure.MongoDb.Repositories;
using SlaytonNichols.Posts.Service.Domain.Documents;

namespace SlaytonNichols.Posts.Service.Core.UseCases.UpdatePostUseCase
{
    public class UpdatePostUseCase : IUpdatePostUseCase
    {
        private readonly IMongoRepository<Post> _posts;
        public UpdatePostUseCase(IMongoRepository<Post> posts)
        {
            _posts = posts;
        }

        public async Task ExecuteAsync(UpdatePostRequest request)
        {
            await _posts.ReplaceOneAsync(request);
        }

    }
}