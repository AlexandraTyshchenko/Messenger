using Messenger.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Messenger.Infrastructure.Configurations
{
    public class UserContactConfiguration : IEntityTypeConfiguration<UserContact>
    {
        public void Configure(EntityTypeBuilder<UserContact> builder)
        {
            builder.Property<Guid>("ContactId");
            builder.Property<Guid>("ContactOwnerId");

            builder.HasOne(uc => uc.Contact)
                .WithMany(u => u.Contacts)
                .HasForeignKey("ContactId")
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(uc => uc.ContactOwner)
                .WithMany()
                .HasForeignKey("ContactOwnerId")
                .OnDelete(DeleteBehavior.NoAction);   
        }
    }
}
