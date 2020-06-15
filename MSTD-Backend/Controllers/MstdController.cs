using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSTD_Backend.Data;
using MSTD_Backend.Enums;
using MSTD_Backend.Interfaces;
using MSTD_Backend.Models.Sources;

namespace MSTD_Backend.Controllers
{
    [ApiController]
    [Route("")]
    public class MstdController : ControllerBase
    {
        private readonly IDictionary<TorrentSource, ITorrentDataSource> _sources;

        public MstdController(IThePirateBaySource thePirateBaySource,
            ILeetxSource leetxSource, IKickassSource kickassSource)
        {
            _sources = new Dictionary<TorrentSource, ITorrentDataSource>
            {
                { TorrentSource.ThePirateBay, thePirateBaySource },
                { TorrentSource.Leetx, leetxSource },
                { TorrentSource.Kickass, kickassSource }
            };
        }

        /// <summary>
        /// Returns a list of torrrent sources for MSTD client app
        /// </summary>
        /// <returns></returns>
        [HttpGet("sources")]
        [ProducesResponseType(typeof(IEnumerable<SourceDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSources()
        {
            return Ok();
        }
    }
}