using MSTD_Backend.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSTD_Backend.Models.Response
{
    public class TorrentSearchResult
    {
        public TorrentSearchResult() { }
        public TorrentSearchResult(IEnumerable<TorrentQueryResult> torrents,
            IEnumerable<ResponseMessage> warnings)
        {
            Torrents = torrents;
            Warnings = warnings;
        }
        public IEnumerable<TorrentQueryResult> Torrents { get; set; }
        public IEnumerable<ResponseMessage> Warnings { get; set; }
    }
}
