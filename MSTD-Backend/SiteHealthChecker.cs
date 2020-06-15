using Microsoft.Extensions.Hosting;
using MSTD_Backend.Interfaces;
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CheckHealth, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            Log.Information("Initialized site health checker");

            return Task.CompletedTask;
        }

        private void CheckHealth(object state)
        {
            Console.WriteLine("Ticked");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            Log.Information("Stopped site health checker");
            return Task.CompletedTask;
        }
    }
}
