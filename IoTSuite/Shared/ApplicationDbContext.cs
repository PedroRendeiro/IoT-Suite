using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTSuite.Shared
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        {
        }
        public DbSet<WeatherForecast> WeatherForecast { get; set; }
        public DbSet<Position> Position { get; set; }
        public DbSet<Thing> Thing { get; set; }
        public DbSet<Policy> Policy { get; set; }
        public DbSet<Measure> Measure { get; set; }
        public DbSet<BasicAuthUser> User { get; set; }

        public DbSet<Alarm> Alarm { get; set; }
        public DbSet<AlarmUser> AlarmUser { get; set; }
        public DbSet<RFIDTag> RFIDTag { get; set; }

        public DbSet<PowerMeterTotal> PowerMeterTotal { get; set; }
        public DbSet<PowerMeterTariff> PowerMeterTariff { get; set; }
        public DbSet<PowerMeterInstantaneous> PowerMeterInstantaneous { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherForecast>().UseXminAsConcurrencyToken();
            modelBuilder.Entity<Position>().UseXminAsConcurrencyToken();
            modelBuilder.Entity<Thing>().UseXminAsConcurrencyToken();
            modelBuilder.Entity<Policy>().UseXminAsConcurrencyToken();
            modelBuilder.Entity<Measure>().UseXminAsConcurrencyToken();
            modelBuilder.Entity<BasicAuthUser>().UseXminAsConcurrencyToken();
            modelBuilder.Entity<Alarm>().UseXminAsConcurrencyToken();
            modelBuilder.Entity<AlarmUser>().UseXminAsConcurrencyToken();
            modelBuilder.Entity<RFIDTag>().UseXminAsConcurrencyToken();
            modelBuilder.Entity<PowerMeterTotal>().UseXminAsConcurrencyToken();
            modelBuilder.Entity<PowerMeterTariff>().UseXminAsConcurrencyToken();
            modelBuilder.Entity<PowerMeterInstantaneous>().UseXminAsConcurrencyToken();

            base.OnModelCreating(modelBuilder);
        }
    }
}
