using EventManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManager.Infrastructure.Persistence.Configurations;

public class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
{
    public void Configure(EntityTypeBuilder<Registration> builder)
    {
        builder.ToTable("registrations");

        builder.HasKey(r => new { r.EventId, r.UserId });

        builder.Property(r => r.EventId)
            .HasColumnName("event_id");

        builder.Property(r => r.UserId)
            .HasColumnName("user_id");

        builder.Property(r => r.RegisteredAt)
            .HasColumnName("registered_at")
            .IsRequired();

        // ── Check-in QR ───────────────────────────────────────────────────
        builder.Property(r => r.CheckInToken)
            .HasColumnName("check_in_token")
            .IsRequired();

        builder.HasIndex(r => r.CheckInToken)
            .IsUnique()
            .HasDatabaseName("IX_registrations_check_in_token");

        builder.Property(r => r.CheckedIn)
            .HasColumnName("checked_in")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(r => r.CheckedInAt)
            .HasColumnName("checked_in_at");
        // ─────────────────────────────────────────────────────────────────

        builder.HasOne(r => r.Event)
            .WithMany(e => e.Registrations)
            .HasForeignKey(r => r.EventId);

        builder.HasOne(r => r.User)
            .WithMany(u => u.Registrations)
            .HasForeignKey(r => r.UserId);
    }
}
