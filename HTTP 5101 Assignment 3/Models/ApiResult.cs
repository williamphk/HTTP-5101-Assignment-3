using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTTP_5101_Assignment_3.Models
{
    public class ApiResult
    {
        public bool Success { get; set; }
        public int? TeacherId { get; set; }
        public string ErrorMsg { get; set; }
    }
}