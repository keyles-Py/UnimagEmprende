using EventManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManager.Infrastructure.Persistence.Configurations;

public class EventFileConfiguration : IEntityTypeConfiguration<EventFile>
{
    public void Configure(EntityTypeBuilder<EventFile> builder)
    {
        builder.ToTable("event_files");

        builder.HasKey(ef => ef.Id);

        builder.Property(ef => ef.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(ef => ef.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.Property(ef => ef.FileName)
            .HasColumnName("file_name")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ef => ef.FileUrl)
            .HasColumnName("file_url")
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(ef => ef.UploadedAt)
            .HasColumnName("uploaded_at")
            .IsRequired();

        builder.Property(ef => ef.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(ef => ef.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasOne(ef => ef.Event)
            .WithMany(e => e.EventFiles)
            .HasForeignKey(ef => ef.EventId);
    }
}
