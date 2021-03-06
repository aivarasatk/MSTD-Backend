﻿using HtmlAgilityPack;
using MSTD_Backend.Constants;
using MSTD_Backend.Data;
using MSTD_Backend.Enums;
using MSTD_Backend.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MSTD_Backend.Services
{
    public class KickassParser : ParserBase, IKickassParser
    {

        public KickassParser()
        {
            DataColumnCount = 6;
        }

        public async Task<string> ParsePageForDescriptionHtmlAsync(string pageContents)
        {
            return await Task.Run(() =>
            {
                var htmlDocument = LoadedHtmlDocument(pageContents);

                var descriptionNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='textcontent']");
                if (descriptionNode == null)
                {
                    Log.Warning("Could not find description node for Kickass");
                    return string.Empty;
                }

                return descriptionNode.InnerHtml;
            });
        }

        public async Task<string> ParsePageForMagnetAsync(string pageContents)
        {
            Log.Information("Kickass magnet parsing parsing");
            return await BaseParseMagnet(pageContents);
        }

        public async Task<TorrentQueryResult> ParsePageForTorrentEntriesAsync(string pageContents)
        {
            return await Task.Run(() =>
            {
                try
                {
                    Log.Information("Kickass parsing");
                    var htmlAgility = LoadedHtmlDocument(pageContents);

                    var tableRows = htmlAgility.DocumentNode.SelectNodes("//tr[@class='odd'] | //tr[@class='even']");//gets table rows that contain torrent data
                    if (NoTableEntries(tableRows))
                        return new TorrentQueryResult { IsLastPage = true };

                    var result = new List<TorrentEntry>();
                    foreach (var row in tableRows)
                    {
                        var columns = row.SelectNodes("td");
                        if (columns == null || columns.Count != DataColumnCount)
                        {
                            Log.Warning($"Could not find all columns for torrent {Environment.NewLine} {row.OuterHtml}");
                            continue;
                        }

                        var titleNode = columns[KickassTorrentIndexer.Name]
                                        .SelectSingleNode("div/div/a[@class='cellMainLink']");
                        if (titleNode == null)
                        {
                            Log.Warning($"Could not find title node for torrent {Environment.NewLine} {columns[KickassTorrentIndexer.Name].OuterHtml}");
                            continue;
                        }

                        var title = titleNode.InnerText.Trim();
                        if (string.IsNullOrEmpty(title))//empty title entry makes no sense. log and skip
                        {
                            Log.Warning($"Empty title from {Environment.NewLine}{titleNode.OuterHtml}");
                            continue;
                        }

                        var torrentUri = titleNode.Attributes.FirstOrDefault(a => a.Name == "href")?.Value;
                        if (string.IsNullOrEmpty(torrentUri))
                        {
                            Log.Warning($"Empty torrent uri from{Environment.NewLine}{titleNode.OuterHtml}");
                            continue;
                        }

                        var magnetLink = string.Empty;

                        if (!int.TryParse(columns[KickassTorrentIndexer.Seeders].InnerText, out var seeders))
                            Log.Warning($"Could not parse seeders {Environment.NewLine}{columns[KickassTorrentIndexer.Seeders].OuterHtml}");

                        if (!int.TryParse(columns[KickassTorrentIndexer.Leechers].InnerText, out var leechers))
                            Log.Warning($"Could not parse leechers {Environment.NewLine}{columns[KickassTorrentIndexer.Leechers].OuterHtml}");

                        var date = columns[KickassTorrentIndexer.Date].InnerText.Trim();

                        var size = columns[KickassTorrentIndexer.Size].InnerText.Trim();
                        var uploader = columns[KickassTorrentIndexer.Uploader].InnerText.Trim();

                        var splitSize = size.Split(' ');
                        result.Add(new TorrentEntry
                        {
                            Title = title,
                            TorrentUri = TrimUriStart(torrentUri),
                            TorrentMagnet = magnetLink,
                            Date = ParseDate(date),
                            Size = new SizeEntity
                            {
                                Value = double.Parse(splitSize[0], CultureInfo.InvariantCulture),
                                Postfix = splitSize[1]
                            },
                            Uploader = uploader,
                            Seeders = seeders,
                            Leechers = leechers,
                            Source = TorrentSource.Kickass
                        });
                    }

                    var pagination = htmlAgility.DocumentNode.SelectSingleNode("//div[@class='pages botmarg5px floatright']");

                    return new TorrentQueryResult
                    {
                        Source = TorrentSource.Kickass,
                        TorrentEntries = result,
                        IsLastPage = IsLastPage(pagination)
                    };

                }
                catch (Exception ex)
                {
                    Log.Warning("Kickass parse exception", ex);
                    throw;
                }
            });
        }

        private DateTime ParseDate(string date)
        {
            var digitEndIndex = 0;
            foreach(var c in date)
            {
                if (char.IsLetter(c))
                    break;
                digitEndIndex++;
            }

            int.TryParse(date.Substring(0, digitEndIndex), out var numberToSubtract);
            var parsedDate = DateTime.UtcNow;

            if (date.Contains("min.")) parsedDate = parsedDate.AddMinutes(-numberToSubtract);
            if (date.Contains("hour")) parsedDate = parsedDate.AddHours(-numberToSubtract);
            if (date.Contains("day")) parsedDate = parsedDate.AddDays(-numberToSubtract);
            if (date.Contains("month")) parsedDate = parsedDate.AddMonths(-numberToSubtract);
            if (date.Contains("year")) parsedDate = parsedDate.AddYears(-numberToSubtract);

            return parsedDate;
        }
        private bool IsLastPage(HtmlNode pagination) => pagination == null || pagination.SelectNodes("a").All(n => n.InnerText != ">>");

        private bool NoTableEntries(HtmlNodeCollection tableRows) => tableRows == null;
    }
}
