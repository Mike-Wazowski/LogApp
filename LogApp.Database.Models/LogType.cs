using Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace LogApp.Database.Models
{
    public class LogType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string HeadersJson { get; set; }
        public virtual ICollection<LogRecord> Records { get; set; }

        public LogType()
        {
            Records = new List<LogRecord>();
        }

        public LogType(string name, string[] headers): this()
        {
            this.Name = name;
            SetHeaders(headers);
        }

        public string[] GetHeaders()
        {
            string[] heders = null;
            string contentString = HeadersJson;
            if (!string.IsNullOrEmpty(contentString))
            {
                try
                {
                    heders = new JavaScriptSerializer().Deserialize<string[]>(contentString);
                }
                catch { }
            }
            if (heders == null)
            {
                heders = new string[0];
            }
            return heders;
        }

        public string SetHeaders(string[] headers)
        {
            string headersJson = string.Empty;
            try
            {
                headersJson = new JavaScriptSerializer().Serialize(headers);
            }
            catch { }
            finally
            {
                if (headersJson == null)
                    headersJson = string.Empty;
                HeadersJson = headersJson;
            }
            return headersJson;
        }
    }
}
