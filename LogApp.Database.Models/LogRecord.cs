using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace LogApp.Database.Models
{
    public class LogRecord
    {
        [Key]
        public int Id { get; set; }
        public string ContentJson { get; set; }
        public int LogTypeId { get; set; }
        [ForeignKey("LogTypeId")]
        public LogType Log { get; set; }

        [NotMapped]
        private string[] Content;

        public string[] GetContent()
        {
            if (Content == null)
            {
                string[] content = null;
                string contentString = ContentJson;
                if (!string.IsNullOrEmpty(contentString))
                {
                    try
                    {
                        content = new JavaScriptSerializer().Deserialize<string[]>(contentString);
                    }
                    catch { }
                }
                if (content == null)
                {
                    content = new string[0];
                }
                Content = content;
                return content;
            }
            else
                return Content;
        }

        public string SetContent(string[] headers)
        {
            string contentJson = string.Empty;
            try
            {
                contentJson = new JavaScriptSerializer().Serialize(headers);
            }
            catch { }
            finally
            {
                if (contentJson == null)
                    contentJson = string.Empty;
                ContentJson = contentJson;
            }
            Content = headers;
            return contentJson;
        }
    }
}
