using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSTD_Backend.Models.Response
{
    public class ErrorMessage
    {
        public ErrorMessage() { }
        public ErrorMessage(string message, string value = "")
        {
            Message = message;
            Value = value;
        }
        public string Message { get; set; }
        public string Value { get; set; }
    }
}
