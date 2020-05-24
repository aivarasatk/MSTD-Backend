using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSTD_Backend.Enums;
using MSTD_Backend.Interfaces;
using MSTD_Backend.Models.Sources;

namespace MSTD_Backend.Controllers
{
    [ApiController]
    [Route("")]
    public class MstdController : ControllerBase
    {
        private readonly ILogService _logger;

        private readonly IDictionary<TorrentSource, ITorrentDataSource> _sources;

        public MstdController(IThePirateBaySource thePirateBaySource, ILogService logger, 
            ILeetxSource leetxSource, IKickassSource kickassSource)
        {
            _sources = new Dictionary<TorrentSource, ITorrentDataSource>
            {
                { TorrentSource.ThePirateBay, thePirateBaySource },
                { TorrentSource.Leetx, leetxSource },
                { TorrentSource.Kickass, kickassSource }
            };
            _logger = logger;
        }

        /// <summary>
        /// Returns a list of torrrent sources for MSTD client app to use for customization
        /// </summary>
        /// <returns></returns>
        [HttpGet("sources")]
        [ProducesResponseType(typeof(IEnumerable<SourceDto>), (int)HttpStatusCode.OK)]
        public IActionResult GetSources()
        {
            throw new NotImplementedException();
        }
    }
}