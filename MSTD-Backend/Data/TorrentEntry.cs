﻿using MSTD_Backend.Enums;
using MSTD_Backend.Models;
using System;

namespace MSTD_Backend.Data
{
    public class TorrentEntry
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public SizeEntity Size { get; set; }
        public string Uploader { get; set; }
        public int Seeders { get; set; }
        public int Leechers{ get; set; }
        public string DescriptionHtml{ get; set; }

        public string TorrentUri { get; set; }
        public string TorrentMagnet { get; set; }

        public TorrentSource Source { get; set; }
    }
}
