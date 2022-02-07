using Microsoft.EntityFrameworkCore;

namespace WeatherMetrics.Models
{
    public class WeatherContext : DbContext
    {
        private readonly string _connectString;

        public WeatherContext(string connectString)
        {
            _connectString = connectString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            optionsBuilder.UseNpgsql(_connectString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Metrics>()
                .ToTable("metrics")
                .HasNoKey();
        }
    }
}
