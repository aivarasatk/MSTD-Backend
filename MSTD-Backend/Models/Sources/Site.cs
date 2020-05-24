using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSTD_Backend.Models.Sources
{
    public class Site
    {
        /// <summary>
        /// Url of the mirror or orgininal site
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Indicates whether Url can be used for queries
        /// </summary>
        public SiteState State { get; set; }
    }
}
