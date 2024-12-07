using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xyz.Models;

namespace Xyz.Infrastructure.EF.UserTokens;

internal class UserTokenConfig : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Token).HasMaxLength(256);
        builder.Property(x => x.UserReferenceId).HasMaxLength(128);
        builder.HasIndex(x => x.UserReferenceId).IsUnique();
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
    }
}