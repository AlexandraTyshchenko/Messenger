using Messenger.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Infrastructure.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.Property<Guid>("UserId");

        builder.HasOne(p => p.User)
            .WithMany(g => g.RefreshTokens)
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
