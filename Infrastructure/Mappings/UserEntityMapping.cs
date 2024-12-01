using Domain.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Mappings
{
    public class UserEntityMapping : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("User");

            builder.HasKey(entity => entity.Id);

            builder.Property(entity => entity.Name).HasMaxLength(250);

            builder.Property(entity => entity.Password).HasMaxLength(250);

            builder.HasMany(x => x.Post)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);
        }

    }
}
