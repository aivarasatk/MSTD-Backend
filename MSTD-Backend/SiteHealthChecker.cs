using Microsoft.Extensions.Hosting;
using MSTD_Backend.Data;
using MSTD_Backend.Enums;
using MSTD_Backend.Interfaces;
using MSTD_Backend.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MSTD_Backend
{
    public class SiteHealthChecker : IHostedService
    {
        private Timer _timer;
        private readonly SourcesHelper _helper;
        private readonly IStateCache _cache;

        public SiteHealthChecker(SourcesHelper helper, IStateCache cache)
        {
            _helper = helper;
            _cache = cache;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CheckHealth, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            Log.Information("Initialized site health checker");

            return Task.CompletedTask;
        }

        private async void CheckHealth(object obj)
        {
            try
            {
                var stateDictionary = new Dictionary<TorrentSource, IEnumerable<SourceState>>();
                foreach(var source in _helper.Sources())
                {
                    var states = new List<SourceState>();
                    await foreach(var state in source.Value.GetSourceStates())
                    {
                        states.Add(state);
                    }
                    stateDictionary.Add(source.Key, states);
                }
                _cache.WriteSourceStates(stateDictionary);
            }
            catch(Exception ex)
            {
                Log.Warning(ex, "Site health check failed");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            Log.Information("Stopped site health checker");
            return Task.CompletedTask;
        }
    }
}
