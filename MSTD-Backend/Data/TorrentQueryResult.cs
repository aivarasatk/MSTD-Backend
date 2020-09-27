using MSTD_Backend.Enums;
using System.Collections.Generic;

namespace MSTD_Backend.Data
{
    public class TorrentQueryResult
    {
        public TorrentQueryResult()
        {
            TorrentEntries = new List<TorrentEntry>();
        }
        public IEnumerable<TorrentEntry> TorrentEntries { get; set; }
        public bool IsLastPage { get; set; }
        public TorrentSource Source { get; set; }
    }
}
