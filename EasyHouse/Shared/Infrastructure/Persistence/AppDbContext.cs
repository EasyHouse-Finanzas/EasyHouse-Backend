using EasyHouse.IAM.Domain.Entities;
using EasyHouse.Simulations.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyHouse.Shared.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // IAM
    public DbSet<User> Users { get; set; }

    // SIMULATIONS
    public DbSet<Client> Clients { get; set; }
    public DbSet<House> Houses { get; set; }
    public DbSet<Config> Configs { get; set; }
    public DbSet<Simulation> Simulations { get; set; }
    public DbSet<Report> Reports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        // USER
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.Number).HasColumnName("phone_number");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Password).HasColumnName("password_hash");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
        });

        // CLIENT
        modelBuilder.Entity<Client>().ToTable("clients").HasKey(c => c.ClientId);

        // HOUSE
        modelBuilder.Entity<House>().ToTable("houses").HasKey(h => h.HouseId);

        // CONFIG
        modelBuilder.Entity<Config>().ToTable("configs").HasKey(c => c.ConfigId);

        // SIMULATION
        modelBuilder.Entity<Simulation>().ToTable("simulations").HasKey(s => s.SimulationId);

        modelBuilder.Entity<Simulation>()
            .HasOne(s => s.Client)
            .WithMany(c => c.Simulations)
            .HasForeignKey(s => s.ClientId);

        modelBuilder.Entity<Simulation>()
            .HasOne(s => s.House)
            .WithMany(h => h.Simulations)
            .HasForeignKey(s => s.HouseId);

        modelBuilder.Entity<Simulation>()
            .HasOne(s => s.Config)
            .WithMany(c => c.Simulations)
            .HasForeignKey(s => s.ConfigId);

        // REPORT
        modelBuilder.Entity<Report>().ToTable("reports").HasKey(r => r.ReportId);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Simulation)
            .WithMany()
            .HasForeignKey(r => r.SimulationId);
    }
}
