using EventManager.Domain.Entities;
using EventManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManager.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        builder.Property(e => e.Location)
            .HasColumnName("location")
            .HasMaxLength(300);

        builder.Property(e => e.StartDate)
            .HasColumnName("start_date")
            .IsRequired();

        builder.Property(e => e.EndDate)
            .HasColumnName("end_date");

        builder.Property(e => e.MaxCapacity)
            .HasColumnName("max_capacity")
            .IsRequired();

        builder.Property(e => e.HasParking)
            .HasColumnName("has_parking")
            .HasDefaultValue(false);

        builder.Property(e => e.ParkingCapacity)
            .HasColumnName("parking_capacity");

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (EventStatus)Enum.Parse(typeof(EventStatus), v));

        builder.Property(e => e.OrganizerId)
            .HasColumnName("organizer_id")
            .IsRequired();

        builder.Property(e => e.ExternalProvider)
            .HasColumnName("external_provider")
            .HasMaxLength(100);

        builder.Property(e => e.ExternalId)
            .HasColumnName("external_id")
            .HasMaxLength(200);

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasOne(e => e.Organizer)
            .WithMany()
            .HasForeignKey(e => e.OrganizerId);

        builder.HasMany(e => e.Registrations)
            .WithOne(r => r.Event)
            .HasForeignKey(r => r.EventId);

        builder.HasMany(e => e.EventFiles)
            .WithOne(ef => ef.Event)
            .HasForeignKey(ef => ef.EventId);

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_events_status");

        builder.HasIndex(e => e.StartDate)
            .HasDatabaseName("IX_events_start_date");

        builder.HasIndex(e => e.OrganizerId)
            .HasDatabaseName("IX_events_organizer_id");
    }
}
