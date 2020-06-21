using MSTD_Backend.Enums;
using MSTD_Backend.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSTD_Backend.Services
{
    public class SourcesHelper
    {
        private readonly IDictionary<TorrentSource, ITorrentDataSource> _sources;

        public SourcesHelper(IThePirateBaySource thePirateBaySource,
            ILeetxSource leetxSource, IKickassSource kickassSource)
        {
            _sources = new Dictionary<TorrentSource, ITorrentDataSource>
            {
                { TorrentSource.ThePirateBay, thePirateBaySource },
                { TorrentSource.Leetx, leetxSource },
                { TorrentSource.Kickass, kickassSource }
            };
        }

        public IDictionary<TorrentSource, ITorrentDataSource> Sources() => _sources;

        public string SourceName(TorrentSource torrentSource) => torrentSource 
            switch
            {
                TorrentSource.ThePirateBay => "The Pirate Bay",
                TorrentSource.Leetx => "1337X",
                TorrentSource.Kickass => "Kickass Torrents"
            };
    }
}
