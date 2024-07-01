using Messenger.Infrastructure.Configurations;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Messenger.Infrastructure.Context
{
    public class ApplicationContext : IdentityDbContext<User, UserRole, Guid>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ParticipantInConversation> ParticipantsInConversation { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new ParticipantInConversationConfiguration());
            modelBuilder.ApplyConfiguration(new ConversationConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var johnId = Guid.NewGuid();
            var janeId = Guid.NewGuid();
            var michaelId = Guid.NewGuid();

            var john = new User
            {
                Id = johnId,
                UserName = "john.doe",
                NormalizedUserName = "JOHN.DOE",
                Email = "john.doe@example.com",
                NormalizedEmail = "JOHN.DOE@EXAMPLE.COM",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "1234567890",
                ImgUrl = "https://example.com/images/john.jpg",
                Bio = "Hello, I'm John.",
                IsActive = true,
                EmailConfirmed = true
            };

            var jane = new User
            {
                Id = janeId,
                UserName = "jane.smith",
                NormalizedUserName = "JANE.SMITH",
                Email = "jane.smith@example.com",
                NormalizedEmail = "JANE.SMITH@EXAMPLE.COM",
                FirstName = "Jane",
                LastName = "Smith",
                PhoneNumber = "9876543210",
                ImgUrl = "https://example.com/images/jane.jpg",
                Bio = "Hi, I'm Jane.",
                IsActive = true,
                EmailConfirmed = true
            };

            var michael = new User
            {
                Id = michaelId,
                UserName = "michael.johnson",
                NormalizedUserName = "MICHAEL.JOHNSON",
                Email = "michael.johnson@example.com",
                NormalizedEmail = "MICHAEL.JOHNSON@EXAMPLE.COM",
                FirstName = "Michael",
                LastName = "Johnson",
                PhoneNumber = "5556667777",
                ImgUrl = "https://example.com/images/michael.jpg",
                Bio = "Hey, I'm Michael.",
                IsActive = true,
                EmailConfirmed = true
            };

            modelBuilder.Entity<User>().HasData(
                john,
                jane,
                michael
            );
        }
    }
}

