using Data.Contexts;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    // Adapted from
    // https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application#creating-the-unit-of-work-class
    public class UnitOfWork : IDisposable
    {
        private readonly MessageBoardsDbContext dbContext = new();

        private GenericRepository<User> users;
        private GenericRepository<MessageBoard> messageBoards;
        private GenericRepository<Message> messages;

        public GenericRepository<User> UsersRepository => this.users ??= new(dbContext);

        public GenericRepository<MessageBoard> MessageBoardsRepository =>
            this.messageBoards ??= new(dbContext);

        public GenericRepository<Message> MessagesRepository => this.messages ??= new(dbContext);

        public void Save() => dbContext.SaveChanges();
        public async Task SaveAsync() => await dbContext.SaveChangesAsync();

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing) dbContext.Dispose();
            }
            this.disposed = true;
        }

        protected virtual async Task DisposeAsync(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing) await dbContext.DisposeAsync();
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            DisposeAsync(true);
            GC.SuppressFinalize(this);
        }
    }
}
