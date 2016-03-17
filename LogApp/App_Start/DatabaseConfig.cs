using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogApp.App_Start
{
    public class DatabaseConfig
    {
        public static void InitializeDatabase()
        {
            System.Data.Entity.Database.SetInitializer(new LogApp.Database.Configuration.Migrations.Initialization.DatabaseInitialization());
        }
    }
}