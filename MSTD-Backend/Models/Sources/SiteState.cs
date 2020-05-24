using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSTD_Backend.Models.Sources
{
    public enum SiteState
    {
        /// <summary>
        /// State indicates that queries can be performed
        /// </summary>
        Active,

        /// <summary>
        /// State indicates that queries CANNOT be performed
        /// </summary>
        Down
    }
}
