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
            IEnumerable<ErrorMessage> errors)
        {
            Torrents = torrents;
            Errors = errors;
        }
        public IEnumerable<TorrentQueryResult> Torrents { get; set; }
        public IEnumerable<ErrorMessage> Errors { get; set; }
    }
}
