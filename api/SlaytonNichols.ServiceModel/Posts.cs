using System;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace SlaytonNichols.ServiceModel
{
    [Description("Blog post")]
    [Notes("Captures a post")]
    public class Post : AuditBase
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string MdText { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }

    [Tag("posts"), Description("Find posts")]
    [Route("/posts", "GET")]
    [Route("/posts/{Id}", "GET")]
    [AutoApply(Behavior.AuditQuery)]
    public class QueryPosts : QueryDb<Post>
    {
        public int? Id { get; set; }
    }

    public class QueryPostsResponse : Post
    {
    }

    [Tag("posts"), Description("Create a new post")]
    [Route("/posts", "POST")]
    [ValidateHasRole("Admin")]
    [AutoApply(Behavior.AuditCreate)]
    public class CreatePost : ICreateDb<Post>, IReturn<IdResponse>
    {
        public int Id { get; set; }
        public string MdText { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }

    [Tag("posts"), Description("Update an existing post")]
    [Route("/posts/{Id}", "PATCH")]
    [ValidateHasRole("Admin")]
    [AutoApply(Behavior.AuditModify)]
    public class UpdatePost : IPatchDb<Post>, IReturn<IdResponse>
    {
        public int Id { get; set; }
        public string MdText { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
