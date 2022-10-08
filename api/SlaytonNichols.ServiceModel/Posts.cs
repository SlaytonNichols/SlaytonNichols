using System;
using System.Collections.Generic;
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
        public string Title { get; set; }
        public string Path { get; set; }
        public string Summary { get; set; }
    }

    [Tag("posts"), Description("Find posts")]
    [Route("/posts", "GET")]
    [Route("/posts/{Path}", "GET")]
    [AutoApply(Behavior.AuditQuery)]
    public class QueryPosts : IQueryDb<Post>, IReturn<IdResponse>
    {
        public string? Path { get; set; }
        int? IQuery.Skip { get; set; }
        int? IQuery.Take { get; set; }
        string IQuery.OrderBy { get; set; }
        string IQuery.OrderByDesc { get; set; }
        string IQuery.Include { get; set; }
        string IQuery.Fields { get; set; }
        Dictionary<string, string> IMeta.Meta { get; set; }
    }

    [Tag("posts"), Description("Create a new post")]
    [Route("/posts", "POST")]
    [ValidateHasRole("Admin")]
    [AutoApply(Behavior.AuditCreate)]
    public class CreatePost : ICreateDb<Post>, IReturn<IdResponse>
    {
        public string MdText { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Summary { get; set; }
    }

    [Tag("posts"), Description("Update an existing post")]
    [Route("/posts/{Id}", "PATCH")]
    [ValidateHasRole("Admin")]
    [AutoApply(Behavior.AuditModify)]
    public class UpdatePost : IPatchDb<Post>, IReturn<IdResponse>
    {
        public int Id { get; set; }
        public string MdText { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Summary { get; set; }
    }

    [Tag("posts"), Description("Delete a Post")]
    [Route("/posts/{Id}", "DELETE")]
    [ValidateHasRole("Admin")]
    [AutoApply(Behavior.AuditSoftDelete)]
    public class DeletePost : IDeleteDb<Post>, IReturnVoid
    {
        public int Id { get; set; }
    }
}
