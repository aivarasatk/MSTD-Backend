using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSTD_Backend.Models.Response
{
    public class DescriptionResponse
    {
        public DescriptionResponse(string description)
        {
            DescriptionHtml = description;
        }
        public string DescriptionHtml { get; set; }

    }
}
