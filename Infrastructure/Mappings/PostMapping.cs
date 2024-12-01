using System.Diagnostics.CodeAnalysis;
using Domain.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mappings
{
    [ExcludeFromCodeCoverage]

    public class PostMapping : IEntityTypeConfiguration<PostEntity>
    {
        public void Configure(EntityTypeBuilder<PostEntity> builder)
        {
            builder.ToTable("Post");

            builder.HasKey(entity => entity.Id);

            builder.Property(entity => entity.Title).HasMaxLength(250);

            builder.Property(entity => entity.Description).HasMaxLength(250);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Post)
                .HasForeignKey(x => x.UserId);


        }

    }
}
