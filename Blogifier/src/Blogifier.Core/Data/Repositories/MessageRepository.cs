using Blogifier.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blogifier.Core.Data.Repositories
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<List<Message>> SelectMessages();
        Task<Message> UpsertMessages(Message message);
    }

    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        AppDbContext _db;

        public MessageRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<List<Message>> SelectMessages() => await Task.FromResult(_db.Messages.ToList());

        public async Task<Message> UpsertMessages(Message message)
        {
            await _db.AddAsync<Message>(message);
            _db.SaveChanges();
            return await Task.FromResult(message);
        }
    }
}
