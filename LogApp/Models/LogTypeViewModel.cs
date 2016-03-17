using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogApp.Models
{
    public class LogTypeViewModel: BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Headers { get; set; }
        public DataTablesConfig Config { get; set; }

        public LogTypeViewModel()
        {
            Headers = new string[0];
            Config = new DataTablesConfig();
        }
    }
}