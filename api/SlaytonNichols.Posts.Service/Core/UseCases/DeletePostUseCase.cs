using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Posts.Service.Domain.Documents;
using SlaytonNichols.Common.Infrastructure.MongoDb.Repositories;

namespace SlaytonNichols.Posts.Service.Core.UseCases.DeletePostUseCase;
public class DeletePostUseCase : IDeletePostUseCase
{
    private readonly IMongoRepository<Post> _posts;
    public DeletePostUseCase(IMongoRepository<Post> posts)
    {
        _posts = posts;
    }

    public async Task ExecuteAsync(DeletePostRequest request)
    {
        await _posts.DeleteOneAsync(x => x.Id == request.Id);
    }

}
