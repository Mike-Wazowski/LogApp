namespace LogApp.Database.Configuration.Migrations.Initialization
{
    using Models;
    using Implementations;
    using System.Collections.Generic;
    using LogApp.Security.Interfaces;
    using LogApp.Security.Implementations;

    public class DatabaseInitialization : System.Data.Entity.CreateDatabaseIfNotExists<LogAppDbContext>
    {
        protected override void Seed(LogAppDbContext context)
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
