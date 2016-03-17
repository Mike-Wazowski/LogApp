using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogApp.Database.Configuration.Implementations
{
    using LogApp.Database.Configuration.Interfaces;
    using LogApp.Database.Models;
    using System.Data.Entity;

    public class LogAppDbContext : DbContext, ILogAppDbContext
    {
        public LogAppDbContext() : base("name=LogAppDbContext") { }

        public DbSet<LogType> LogTypes { get; set; }
        public DbSet<LogRecord> LogRecords { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogType>()
                .HasMany<LogRecord>(lt => lt.Records)
                .WithRequired(lr => lr.Log)
                .HasForeignKey(lr => lr.LogTypeId);
        }
    }
}
