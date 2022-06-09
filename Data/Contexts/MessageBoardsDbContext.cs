using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contexts
{
    public class MessageBoardsDbContext : DbContext
    {
        private const string CONNECTION_STRING =
            @"Server=.\SQLExpress;Database=MessageBoardsDB;Integrated Security=SSPI";
        private const string SECRET_KEY = "ZRTeH3zO6FcF7wYUW1bAHIFlSMbJOn7zpQtzoaQ60Mgo09xpRZcf4syOJKw5wBzlabboOpy6H-B1JnnvchEDh5lK2Kpyl8gDfxwR9w56MVMJJUNhtb7qLIx5lC2MuQxqEJqTjmuenMKoc21vtOqOjNnGStmY7u85-zXXADW4rdk";
        HMACSHA256 sha256Hasher = new(Encoding.UTF8.GetBytes(SECRET_KEY));

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageBoard> MessageBoards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(CONNECTION_STRING);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var password = "ADMINADMIN";
            using var passwordStream = new MemoryStream(Encoding.UTF8.GetBytes(password));
            var hashedPassword = Encoding.UTF8.GetString(sha256Hasher.ComputeHash(passwordStream));

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FirstName = "ADMIN",
                LastName = "ADMIN",
                UserName = "ADMIN",
                Password = hashedPassword,
                IsAdmin = true,
            });
        }
    }
}
