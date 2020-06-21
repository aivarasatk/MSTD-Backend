using MSTD_Backend.Data;
using MSTD_Backend.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSTD_Backend.Interfaces
{
    public interface IStateCache
    {
        Task<Dictionary<TorrentSource, IEnumerable<SourceState>>> SourceStatesAsync();
        void WriteSourceStates(Dictionary<TorrentSource, IEnumerable<SourceState>> states);
    }
}
