

namespace MyApp.Models;

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<AppRoom> Rooms => Set<AppRoom>();
    public DbSet<AppMessage> Messages => Set<AppMessage>();
    public DbSet<AppUserConnection> UserConnections => Set<AppUserConnection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // AppUser → AppUserConnection (One-to-Many)
        modelBuilder.Entity<AppUserConnection>()
            .HasOne(c => c.User)
            .WithMany(u => u.Connections)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // AppUser → AppMessage (One-to-Many) for sent messages
        modelBuilder.Entity<AppMessage>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // AppRoom → AppMessage (One-to-Many)
        modelBuilder.Entity<AppMessage>()
            .HasOne(m => m.Room)
            .WithMany(r => r.Messages)
            .HasForeignKey(m => m.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        // AppRoom → User1 (Many-to-One)
        modelBuilder.Entity<AppRoom>()
            .HasOne(r => r.User1)
            .WithMany()
            .HasForeignKey("UserId1")
            .OnDelete(DeleteBehavior.Restrict);

        // AppRoom → User2 (Many-to-One)
        modelBuilder.Entity<AppRoom>()
            .HasOne(r => r.User2)
            .WithMany()
            .HasForeignKey("UserId2")
            .OnDelete(DeleteBehavior.Restrict);

        // Ensure one room per pair of users (unique constraint)
        modelBuilder.Entity<AppRoom>()
            .HasIndex("UserId1", "UserId2")
            .IsUnique();

        // Ensure ConnectionId is unique
        modelBuilder.Entity<AppUserConnection>()
            .HasIndex(c => c.ConnectionId)
            .IsUnique();
    }
}
