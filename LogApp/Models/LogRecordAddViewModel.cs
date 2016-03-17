using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogApp.Models
{
    public class LogRecordAddViewModel
    {
        public string[] Headers { get; set; }
        public List<string[]> Records { get; set; }

        public LogRecordAddViewModel()
        {
            Headers = new string[0];
            Records = new List<string[]>();
        }
    }
}