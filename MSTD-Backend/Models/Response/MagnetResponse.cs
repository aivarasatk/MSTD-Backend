using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSTD_Backend.Models.Response
{
    public class MagnetResponse
    {
        public MagnetResponse(string magnet)
        {
            Magnet = magnet;
        }

        public string Magnet { get; set; }        
    }
}
