using Microsoft.Extensions.Configuration;
using MSTD_Backend.Data;
using MSTD_Backend.Enums;
using MSTD_Backend.Interfaces;
using MSTD_Backend.Mapping;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace MSTD_Backend.Services
{
    public class LeetxSource : SourceBase, ILeetxSource
    {
        private readonly ILogService _logger;
        private readonly ILeetxParser _parser;

        private string _categorySearchResource;
        private string _categorySearchEndpoint;

        public LeetxSource(ILogService logger, ILeetxParser parser, IConfiguration config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));

            _baseUrl = config.GetValue<string>("UrlInfo:LeetxUrl");
            _searchResource = config.GetValue<string>("UrlInfo:LeetxSearchEndpoint");
            _searchEndpoint = Path.Combine(_baseUrl, _searchResource);

            _categorySearchResource = config.GetValue<string>("UrlInfo:LeetxCategorySearchEndpoint");
            _categorySearchEndpoint = Path.Combine(_baseUrl, _categorySearchResource); 
        }

        public void UpdateUsedSource(string newBaseUrl)
        {
            BaseUpdateUsedSource(newBaseUrl);
            _categorySearchEndpoint = Path.Combine(_baseUrl, _categorySearchResource);
        }

        public IEnumerable<string> GetSources()
        {
            return BaseGetSources();
        }

        public async Task<TorrentQueryResult> GetTorrentsAsync(string searchFor, int page, Sorting sorting)
        {
            var mapperSorting = SortingMapper.SortingToLeetxSorting(sorting);
            var fullUrl = Path.Combine(_searchEndpoint, searchFor, mapperSorting.SortedBy, mapperSorting.Order, page.ToString()) + Path.DirectorySeparatorChar;
            var contents = await UrlGetResponseString(fullUrl);
            return await _parser.ParsePageForTorrentEntriesAsync(contents);
        }

        public async Task<string> GetTorrentMagnetAsync(string detailsUri)
        {
            return await BaseGetTorrentMagnetAsync(detailsUri, _parser);
        }

        public async Task<string> GetTorrentDescriptionAsync(string detailsUri)
        {
            return await BaseGetTorrentDescriptionAsync(Path.Combine(_baseUrl, detailsUri), _parser);
        }

        public async Task<TorrentQueryResult> GetTorrentsByCategoryAsync(string searchFor, int page, Sorting sorting, TorrentCategory category)
        {
            var mapperSorting = SortingMapper.SortingToLeetxSorting(sorting);
            var mappedCategory = TorrentCategoryMapper.ToLeetxCategory(category);

            var fullUrl = Path.Combine(_categorySearchEndpoint, searchFor, mappedCategory, mapperSorting.SortedBy, mapperSorting.Order, page.ToString()) + Path.DirectorySeparatorChar;
            var contents = await UrlGetResponseString(fullUrl);
            return await _parser.ParsePageForTorrentEntriesAsync(contents);
        }

        public string FullTorrentUrl(string uri) => TorrentUrl(uri);

        public async IAsyncEnumerable<SourceState> GetSourceStates()
        {
            await foreach (var source in BaseGetSourceStates(() => GetTorrentsAsync(searchFor: "demo", page: 1, Sorting.SeedersDesc)))
                yield return source;
        }
    }
}
