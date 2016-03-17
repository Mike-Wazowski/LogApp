using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogApp.Models
{
    public class BaseModel
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public string SuccessMessage { get; set; }

        public void SetError(string message)
        {
            HasError = true;
            ErrorMessage = message;
        }

        public void SetSuccess(string message)
        {
            Success = true;
            SuccessMessage = message;
        }
    }
}