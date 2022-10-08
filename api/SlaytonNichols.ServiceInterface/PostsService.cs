using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Logging;
using SlaytonNichols.ServiceModel;

namespace SlaytonNichols.ServiceInterface;

public class PostsServices : Service
{
    public IAutoQueryDb AutoQuery { get; set; }
    // private readonly ILogger<PostsServices> _logger;

    // public PostsServices(ILogger<PostsServices> logger)
    // {
    //     _logger = logger;
    // }

    // Async
    public async Task<object> Get(QueryPosts query)
    {
        var q = AutoQuery.CreateQuery(query, base.Request);
        var response = await AutoQuery.ExecuteAsync(query, q, base.Request);
        return response;
    }

    public async Task<object> Post(CreatePost query)
    {
        var response = await AutoQuery.CreateAsync<Post>(query, base.Request);
        return response;
    }

    public async Task<object> Patch(UpdatePost query)
    {
        var response = await AutoQuery.PatchAsync<Post>(query, base.Request);
        return response;
    }

    public async Task Delete(DeletePost query)
    {
        _ = await AutoQuery.DeleteAsync<Post>(query, base.Request);
    }
}
