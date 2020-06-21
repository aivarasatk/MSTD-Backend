using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSTD_Backend.Data
{
    public class SourceState
    {
        public SourceState(string site, bool isAlive)
        {
            SiteName = site;
            IsAlive = isAlive;
        }

        public string SiteName { get; private set; }
        public bool IsAlive{ get; private set; }
    }
}
