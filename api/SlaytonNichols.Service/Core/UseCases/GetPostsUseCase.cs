using SlaytonNichols.Service.Infrastructure.MongoDb.Entities;
using SlaytonNichols;
using MongoDB.Driver;
using SlaytonNichols.Service.Core.Dtos;
using SlaytonNichols.Service.Infrastructure.MongoDb.Repositories;

namespace SlaytonNichols.Service.Core.UseCases.GetPostsUseCase
{
    public class GetPostsUseCase : IGetPostsUseCase
    {
        private readonly IMongoRepository<Post> _posts;
        public GetPostsUseCase(IMongoRepository<Post> posts)
        {
            _posts = posts;
        }

        public async Task<IEnumerable<Post>> ExecuteAsync(GetPostsRequest request)
        {
            var posts = _posts.FilterBy(x => x.Path == request.Path || string.IsNullOrEmpty(request.Path));
            return posts.ToList();
        }
    }
}