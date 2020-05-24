using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSTD_Backend.Models.Sources
{
    public class SourceDto
    {
        /// <summary>
        /// UI friendly source name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Enum idenfying the source
        /// </summary>
        public TorrenttSource UniqueId { get; set; }

        /// <summary>
        /// A list of possible Urls and their availability for queries
        /// </summary>
        public IEnumerable<Site> Sites { get; set; }
    }
}
