using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xyz.Models;

namespace Xyz.Infrastructure.EF.Users;

internal class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Email).HasMaxLength(50);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.FullName).HasMaxLength(50);
        builder.Property(x => x.Password).HasMaxLength(128);
        builder.Property(x => x.PasswordSalt).HasMaxLength(128);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.HasIndex(x => x.PhoneNumber).IsUnique();
    }
}