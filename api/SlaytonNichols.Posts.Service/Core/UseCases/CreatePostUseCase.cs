
using MongoDB.Driver;
using SlaytonNichols.Posts.Service.Core.Dtos;
using MongoDB.Bson;
using SlaytonNichols.Posts.Service.Domain.Documents;
using SlaytonNichols.Common.Infrastructure.MongoDb.Repositories;

namespace SlaytonNichols.Posts.Service.Core.UseCases.CreatePostUseCase
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