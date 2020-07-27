using Blogifier.Core.Data;
using Blogifier.Core.Data.Repositories;
using System;

namespace Blogifier.Core.Services
{
    public interface IDataService : IDisposable
    {
        IPostRepository BlogPosts { get; }
        IAuthorRepository Authors { get; }
        INotificationRepository Notifications { get; }        
        ICustomFieldRepository CustomFields { get; }
        INewsletterRepository Newsletters { get; }        
        IMessageRepository Message { get; }

        int Complete();
    }

    public class DataService : IDataService
    {
        private readonly AppDbContext _db;

        public DataService(
            AppDbContext db,
            IPostRepository postRepository,
            IAuthorRepository authorRepository,
            INotificationRepository notificationRepository,            
            ICustomFieldRepository customFieldRepository,
            INewsletterRepository newsletterRepository,
            IMessageRepository messageRepository)
        {
            _db = db;

            BlogPosts = postRepository;
            Authors = authorRepository;
            Notifications = notificationRepository;            
            CustomFields = customFieldRepository;
            Newsletters = newsletterRepository;
            Message = messageRepository;
        }

        public IPostRepository BlogPosts { get; private set; }
        public IAuthorRepository Authors { get; private set; }
        public INotificationRepository Notifications { get; private set; }        
        public ICustomFieldRepository CustomFields { get; private set; }
        public INewsletterRepository Newsletters { get; private set; }
        public IMessageRepository Message { get; set; }


        public int Complete()
        {
            return _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}