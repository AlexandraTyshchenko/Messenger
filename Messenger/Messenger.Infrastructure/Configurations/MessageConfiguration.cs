using Messenger.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Infrastructure.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.Property<Guid?>("SenderId");

        builder.HasOne(m => m.Sender)
            .WithMany(c => c.Messages)
            .HasForeignKey("SenderId")
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property<Guid>("ConversationId");

        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey("ConversationId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property<Guid?>("ImageId");

        builder.HasOne(m => m.Image)
           .WithOne(c => c.Message)
           .HasForeignKey<Message>("ImageId")
           .OnDelete(DeleteBehavior.SetNull)
           .IsRequired(false);
        
    }
}
