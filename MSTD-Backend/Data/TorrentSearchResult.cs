using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSTD_Backend.Data
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
