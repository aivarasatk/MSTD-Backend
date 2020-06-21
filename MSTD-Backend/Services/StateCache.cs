using MSTD_Backend.Data;
using MSTD_Backend.Enums;
using MSTD_Backend.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MSTD_Backend.Services
{
    public class StateCache : IStateCache
    {
        private const int ChannelCapacity = 1;
        private readonly Channel<Dictionary<TorrentSource, IEnumerable<SourceState>>> _channel;
        private Dictionary<TorrentSource, IEnumerable<SourceState>> _states;

        public StateCache()
        {
            _channel = Channel.CreateBounded<Dictionary<TorrentSource, IEnumerable<SourceState>>>(new BoundedChannelOptions(ChannelCapacity)
            {
                SingleReader = true,
                SingleWriter = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        }

        public async Task<Dictionary<TorrentSource, IEnumerable<SourceState>>> SourceStatesAsync()
        {
            if (_states is null)
                _states = await _channel.Reader.ReadAsync();
            else
                _channel.Reader.TryRead(out _states);//we don't expect frequent changes in source availabilities. Failing to read is Ok

            return _states;
        }

        public void WriteSourceStates(Dictionary<TorrentSource, IEnumerable<SourceState>> states)
        {
            _channel.Writer.TryWrite(states);
        }
    }
}
