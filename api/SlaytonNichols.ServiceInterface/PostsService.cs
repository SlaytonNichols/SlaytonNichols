using System;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack;
using ServiceStack.Logging;
using SlaytonNichols.ServiceModel;

namespace SlaytonNichols.ServiceInterface;

public class PostsServices : Service
{
    public IAutoQueryDb AutoQuery { get; set; }
    public static ILog Log = LogManager.GetLogger(typeof(PostsServices));

    // Async
    public async Task<QueryResponse<Post>> Get(QueryPosts query)
    {
        Log.Info(base.Request);
        using var db = AutoQuery.GetDb(query, base.Request);
        var q = AutoQuery.CreateQuery(query, base.Request, db);
        var response = await AutoQuery.ExecuteAsync(query, q, base.Request, db);
        return response;
    }
}
