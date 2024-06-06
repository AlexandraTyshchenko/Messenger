using Messenger.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Context
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
        public DbSet<Message> Messages { get; set; }
    }
}
