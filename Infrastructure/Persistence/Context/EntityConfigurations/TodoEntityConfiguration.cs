using Domain.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Context.EntityConfigurations
{
    internal class TodoEntityConfiguration : IEntityTypeConfiguration<Todo>
    {
        public void Configure(EntityTypeBuilder<Todo> builder)
        {
            // Table name
            builder.ToTable("Todos");

            // Primary key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Title)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.Status)
                .IsRequired();

            // Auditable properties
            builder.Property(t => t.CreatedBy)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(t => t.Created)
                .IsRequired();

            builder.Property(t => t.LastModifiedBy)
                .HasMaxLength(50);

            // Soft delete configuration
            builder.Property(t => t.DeFlag)
                .IsRequired()
                .HasDefaultValue(false);

            // Indexes
            builder.HasIndex(t => t.Title); // Index on Title

            builder.HasIndex(t => t.Status); // Index on Status
        }
    }
}
