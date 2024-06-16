using Messenger.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Messenger.Infrastructure.Configurations
{
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.Property<Guid?>("GroupId");

            builder.HasOne(g => g.Group)
                .WithOne(c => c.Conversation)
                .HasForeignKey<Conversation>("GroupId")
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
}
