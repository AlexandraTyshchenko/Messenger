using Messenger.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Context
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserContact> UserContacts { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserContact>()
             .HasOne(uc => uc.Contact)
             .WithMany(u => u.Contacts)
             .HasForeignKey(uc => uc.UserContactId)
             .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserContact>()
                .HasOne(uc => uc.ContactOwner)
                .WithMany()
                .HasForeignKey(uc => uc.UserContactOwnerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ParticipentInConversation>()
                .HasOne(p => p.User)
                .WithMany(u => u.ParticipentInConversation)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<ParticipentInConversation>()
                .HasOne(p => p.Conversation)
                .WithMany(g => g.ParticipantsInGroup)
                .HasForeignKey(p => p.ConversationId);

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Conversation)
                .WithOne(c => c.Group)
                .HasForeignKey<Conversation>(c => c.GroupId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ParticipentInConversationId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

        }  
    }
}
