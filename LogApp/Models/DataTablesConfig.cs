using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogApp.Models
{
    public class DataTablesConfig
    {
        public bool bServerSide { get; set; }
        public string sAjaxSource { get; set; }
        public bool bProcessing { get; set; }
        public object[][] order { get; set; }
        public object[] aoColumns { get; set; }
        public object language { get; set; }

        public DataTablesConfig()
        {
            bServerSide = true;
            sAjaxSource = "LogRecordAjaxSource";
            bProcessing = true;
            order = new object[1][];
            order[0] = new object[2];
            order[0][0] = 0;
            order[0][1] = "desc";
            language = new DataTablesUrl() { url = "../Scripts/plugins/dataTables/Polish.json" };
        }
    }
}