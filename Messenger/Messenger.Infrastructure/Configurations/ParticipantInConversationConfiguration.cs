using Messenger.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Messenger.Infrastructure.Configurations
{
    public class ParticipantInConversationConfiguration : IEntityTypeConfiguration<ParticipantInConversation>
    {
        public void Configure(EntityTypeBuilder<ParticipantInConversation> builder)
        {
            builder.Property<Guid>("UserId");
            builder.Property<Guid>("ConversationId");

            builder.HasOne(p => p.User)
                .WithMany(u => u.ParticipantsInConversation)
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Conversation)
                .WithMany(g => g.ParticipantsInConversation)
                .HasForeignKey("ConversationId")
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
