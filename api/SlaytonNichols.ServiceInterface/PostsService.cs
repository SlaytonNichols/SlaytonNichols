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
    private readonly ILogger<PostsServices> _logger;

    public PostsServices(ILogger<PostsServices> logger)
    {
        _logger = logger;
    }

    // Async
    public async Task<QueryResponse<Post>> Get(QueryPosts query)
    {
        _logger.LogInformation("Test Log");
        using var db = AutoQuery.GetDb(query, base.Request);
        var q = AutoQuery.CreateQuery(query, base.Request, db);
        var response = await AutoQuery.ExecuteAsync(query, q, base.Request, db);
        return response;
    }
}
