using SlaytonNichols.Posts.Service.Core.Dtos;
using SlaytonNichols.Common.Infrastructure.MongoDb.Repositories;
using SlaytonNichols.Posts.Service.Infrastructure.Data.Documents;

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
        await _posts.DeleteByIdAsync(request.Id);
    }

}
