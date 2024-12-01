using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Mappings
{
    [ExcludeFromCodeCoverage]

    public class AuditLogEntityMapping : IEntityTypeConfiguration<AuditLogEntity>
    {
        public void Configure(EntityTypeBuilder<AuditLogEntity> builder)
        {
            builder.ToTable("Auditlog");

            builder.HasKey(entity => entity.Id);

            builder.Property(entity => entity.TableName).HasMaxLength(250);

            builder.Property(entity => entity.ActionType).HasMaxLength(250);

            builder.Property(entity => entity.EntityId).HasMaxLength(250);


            builder.Property(entity => entity.User).HasMaxLength(250);

            builder.Property(entity => entity.Timestamp);



        }
    }

}