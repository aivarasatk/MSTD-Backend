using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSTD_Backend.Data;
using MSTD_Backend.Enums;
using MSTD_Backend.Interfaces;
using MSTD_Backend.Models.Response;
using MSTD_Backend.Models.Sources;
using MSTD_Backend.Services;
using Serilog;

namespace MSTD_Backend.Controllers
{
    [ApiController]
    [Route("")]
    public class MstdController : ControllerBase
    {
        private readonly SourcesHelper _helper;

        private readonly IStateCache _cache;
        private readonly ILogger _logger;

        public MstdController(SourcesHelper helper, IStateCache cache, ILogger logger)
        {
            _helper = helper;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Returns a list of torrrent sources for MSTD client app
        /// </summary>
        /// <returns></returns>
        [HttpGet("sources")]
        [ProducesResponseType(typeof(IEnumerable<SourceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSourcesAsync()
        {
            var sources = await _cache.SourceStatesAsync();

            var result = sources.Select(s => new SourceDto
            {
                Name = _helper.SourceName(s.Key),
                UniqueId = s.Key,
                Sites = s.Value.Select(s => new Site
                {
                    State = s.IsAlive ? SiteState.Active : SiteState.Down,
                    Url = s.SiteName
                })
            });
            return Ok(result);
        }

        /// <summary>
        /// Provides access to torrent search based on provided parameters
        /// </summary>
        /// <param name="urls">Urls to use for search. Invalid urls will not be used</param>
        /// <param name="sortOrder"></param>
        /// <param name="category">In which torrent category to search</param>
        /// <param name="searchValue">Text that is used to query torrents</param>
        /// <param name="page">Current search page. Must be greater than 0</param>
        /// <returns></returns>
        [HttpGet("torrents")]
        [ProducesResponseType(typeof(TorrentSearchResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTorrentsAsync(
            [FromQuery][Required] ICollection<string> urls,
            [FromQuery][Required] Sorting sortOrder, 
            [FromQuery][Required] TorrentCategory category,
            [FromQuery][Required] string searchValue,
            [FromQuery][Required] int page)
        {
            if (page <= 0) 
                return BadRequest(ErrorResponse(new ResponseMessage(message: "Invalid page number", value: page.ToString())));

            var validUrls = ValidUrls(urls).ToArray();
            if (!validUrls.Any()) 
                return BadRequest(ErrorResponse(new ResponseMessage(message: "Provided urls are invalid")));

            var mappedUrls = MapValidUrls(validUrls);
            var torrentResults = await ExecuteTorrentSearch(mappedUrls, searchValue, category, sortOrder, page);

            return Ok(new TorrentSearchResult
            {
                Torrents = torrentResults,
                Warnings = urls.Except(validUrls).Select(u => new ResponseMessage(message: "Invalid Url", value: u))
            });
        }

        [HttpGet("magnet")]
        [ProducesResponseType(typeof(MagnetResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMagnetAsync(
            [Required][FromQuery] string baseUrl,
            [Required][FromQuery] string torrentPath,
            [Required][FromQuery] TorrentSource source)
        {
            var dataSource = _helper.Sources()[source];

            if (!dataSource.GetSources().Contains(baseUrl))
                return BadRequest(new ResponseMessage("Base url is not part of known values for this source", baseUrl));

            dataSource.UpdateUsedSource(baseUrl);

            try
            {
                var response = await dataSource.GetTorrentMagnetAsync(torrentPath);
                return Ok(new MagnetResponse(response));
            }
            catch(Exception ex)
            {
                var fullUrl = Path.Combine(baseUrl, torrentPath);
                _logger.Information(ex, $"Error retrieving magnet for {source} {fullUrl}");
                return NotFound(new ResponseMessage("Magnet not found", fullUrl));
            }
        }

        private IEnumerable<ResponseMessage> ErrorResponse(ResponseMessage errorMessage) => new[] { errorMessage };

        private async Task<IEnumerable<TorrentQueryResult>> ExecuteTorrentSearch(IEnumerable<KeyValuePair<TorrentSource, string>> mappedUrls, 
            string searchValue, TorrentCategory category, Sorting sortOrder, int page)
        {
            var res = new List<TorrentQueryResult>();
            foreach (var url in mappedUrls)
            {
                var source = _helper.Sources()[url.Key];

                source.UpdateUsedSource(url.Value);//TODO: refactor how searching works 

                var torrents = category == TorrentCategory.All
                    ? await source.GetTorrentsAsync(searchValue, page, sortOrder)
                    : await source.GetTorrentsByCategoryAsync(searchValue, page, sortOrder, category);

                res.Add(torrents);
            }
            return res;
        }

        private IEnumerable<KeyValuePair<TorrentSource, string>> MapValidUrls(IEnumerable<string> validUrls)
        {
            foreach(var url in validUrls)
            {
                foreach (var source in _helper.Sources())
                {
                    if (source.Value.GetSources().Contains(url))
                    {
                        yield return new KeyValuePair<TorrentSource, string>(source.Key, url);
                        break;
                    }
                }
            }
        }

        private IEnumerable<string> ValidUrls(ICollection<string> urls)
        {
            var sites = _helper.Sources().SelectMany(s => s.Value.GetSources());
            return urls.Where(u => sites.Any(s => s == u));
        }

    }
}