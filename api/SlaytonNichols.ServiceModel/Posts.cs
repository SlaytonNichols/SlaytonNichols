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

    [Tag("bookings"), Description("Create a new post")]
    [Route("/bookings", "POST")]
    [ValidateHasRole("Employee")]
    [AutoApply(Behavior.AuditCreate)]
    public class CreatePost : ICreateDb<Post>, IReturn<IdResponse>
    {
        public int Id { get; set; }
        public string MdText { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }

    [Tag("bookings"), Description("Update an existing post")]
    [Route("/booking/{Id}", "PATCH")]
    [ValidateHasRole("Employee")]
    [AutoApply(Behavior.AuditModify)]
    public class UpdatePost : IPatchDb<Post>, IReturn<IdResponse>
    {
        public int Id { get; set; }
        public string MdText { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
