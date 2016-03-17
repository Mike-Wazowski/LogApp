namespace LogApp.Database.Configuration.Migrations
{
    using Security.Interfaces;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Security.Implementations;

    internal sealed class Configuration : DbMigrationsConfiguration<LogApp.Database.Configuration.Implementations.LogAppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "LogApp.Database.Configuration.Implementations.LogAppDbContext";
        }

        protected override void Seed(LogApp.Database.Configuration.Implementations.LogAppDbContext context)
        {
            IPasswordManager encryptor = new PasswordManager();
            User admin = new User() { Login = "admin" };
            string hashedPassword = encryptor.EncryptPassword("ZST_LogApp");
            string salt = encryptor.GenerateSalt();
            admin.Password = encryptor.EncryptPassword(hashedPassword, salt);
            admin.Salt = salt;
            context.Users.Add(admin);
            context.SaveChanges();
        }
    }
}
