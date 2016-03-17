namespace LogApp.Database.Configuration.Interfaces
{
    using LogApp.Database.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public interface ILogAppDbContext
    {
        DbSet<LogType> LogTypes { get; set; }
        DbSet<LogRecord> LogRecords { get; set; }
        DbSet<User> Users { get; set; }

        System.Data.Entity.Database Database { get; }

        DbEntityEntry Entry(object entity);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        int SaveChanges();

        DbSet Set(Type entityType);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        DbContextConfiguration Configuration { get; }
    }
}
