using Microsoft.EntityFrameworkCore;

namespace WeatherMetrics.Models
{
    public class WeatherContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            optionsBuilder.UseNpgsql("Server=localhost;Port=54321;Database=WeatherMetrics;User Id=postgres;Password=@Passw0rd;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Metrics>()
                .ToTable("metrics")
                .HasNoKey();
        }
    }
}
