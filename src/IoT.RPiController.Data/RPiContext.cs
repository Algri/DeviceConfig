using IoT.RPiController.Data.Configuration;
using IoT.RPiController.Data.Entities;
using IoT.RPiController.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace IoT.RPiController.Data
{
    public class RPiContext : DbContext
    {
        public RPiContext(DbContextOptions<RPiContext> options) : base(options)
        {
            // Database.EnsureDeleted();

            Database.EnsureCreated();
        }

        public virtual DbSet<Module> ModuleConfigurations { get; set; }
        public virtual DbSet<GeneralConfiguration> GeneralConfigurations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");

            modelBuilder.Entity<TimerValue>().HasIndex(x => x.ModuleId);

            modelBuilder.Entity<TimerValue>()
                .HasAlternateKey(tv => new { tv.RelayNumber, tv.ModuleId });

            modelBuilder.Entity<RelayInfo>().HasIndex(x => x.ModuleId);

            modelBuilder.Entity<RelayInfo>()
                .HasAlternateKey(ri => new { ri.RelayNumber, ri.ModuleId });
            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<GeneralConfiguration>()
                .HasIndex(c => c.Key)
                .IsUnique();

            modelBuilder.ApplyConfiguration(new SeedModuleConfigurations());

            modelBuilder.ApplyConfiguration(new SeedUsers());

            modelBuilder.Entity<GeneralConfiguration>().HasData(
                new GeneralConfiguration
                {
                    Id = 1,
                    Key = ConfigurationKeysEnum.NodeRedUrl.ToString(),
                    Type = typeof(string).FullName!,
                    Value = string.Empty
                });
        }
    }
}