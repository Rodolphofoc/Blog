using System.Diagnostics.CodeAnalysis;
using Domain.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mappings
{
    [ExcludeFromCodeCoverage]
    public class EntityMapping<TEntity> : IEntityTypeConfiguration<TEntity>
       where TEntity : Entity
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(entity => entity.Id);

            builder.Property(e => e.CreatedAt);
            builder.Property(e => e.CreatedBy);
            builder.Property(e => e.LastModified);
            builder.Property(e => e.LastModifiedBy);

            builder.Property(entity => entity.Id)
                .HasDefaultValueSql("NewId()");

        }
    }
}
