using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogApp.Models
{
    public class LogRecordViewModel: BaseModel
    {
        public int Id { get; set; }
        public string[] Attributes { get; set; }
    }
}