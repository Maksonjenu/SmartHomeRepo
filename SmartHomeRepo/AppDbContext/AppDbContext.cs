using Microsoft.EntityFrameworkCore;
using SmartHomeRepo.Entitys;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Apartment> Apartments => Set<Apartment>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomInfo> RoomInfos => Set<RoomInfo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // Связь 1 к 1 между Room и RoomInfo
        modelBuilder.Entity<Room>()
            .HasOne(r => r.Info)
            .WithOne(i => i.Room)
            .HasForeignKey<RoomInfo>(i => i.RoomId);
    }
}