using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VideoCollabServer.Models;


namespace VideoCollabServer.Data;

public sealed class ApplicationContext : IdentityDbContext<User>
{
    public DbSet<Link> Links { get; set; } = null!;
    public DbSet<Movie> Movies { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>()
            .HasMany(u => u.ConnectedRooms)
            .WithMany(r => r.JoinedUsers)
            .UsingEntity(j => j.ToTable("RoomUsers"))
            .HasMany(u => u.OwnedRooms)
            .WithOne(r => r.Owner);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.VideoOperator)
            .WithMany();
        
        modelBuilder.Entity<Movie>()
            .Property(m => m.Status)
            .HasConversion<string>();
    }
}