using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSTD_Backend.Enums;
using MSTD_Backend.Interfaces;
using MSTD_Backend.Models.Sources;
using MSTD_Backend.Services;

namespace MSTD_Backend.Controllers
{
    [ApiController]
    [Route("")]
    public class MstdController : ControllerBase
    {
        private readonly IDictionary<TorrentSource, ITorrentDataSource> _sources;
        private readonly SourcesHelper _helper;

        private readonly IStateCache _cache;

        public MstdController(SourcesHelper helper, IStateCache cache)
        {
            _helper = helper;
            _cache = cache;
        }

        /// <summary>
        /// Returns a list of torrrent sources for MSTD client app
        /// </summary>
        /// <returns></returns>
        [HttpGet("sources")]
        [ProducesResponseType(typeof(IEnumerable<SourceDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSources()
        {
            var sources = await _cache.SourceStatesAsync();

            var result = new List<SourceDto>();
            foreach(var source in sources)
            {
                result.Add(new SourceDto
                {
                    Name = _helper.SourceName(source.Key),
                    UniqueId = source.Key,
                    Sites = source.Value.Select(s => new Site
                    {
                        State = s.IsAlive ? SiteState.Active : SiteState.Down,
                        Url = s.SiteName
                    })
                });
            }
            return Ok(result);
        }
    }
}