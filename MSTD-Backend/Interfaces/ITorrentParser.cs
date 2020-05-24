using MSTD_Backend.Data;
using System.Threading.Tasks;

namespace MSTD_Backend.Interfaces
{
    public interface ITorrentParser
    {
        Task<TorrentQueryResult> ParsePageForTorrentEntriesAsync(string pageContents);
        Task<string> ParsePageForMagnetAsync(string pageContents);
        Task<string> ParsePageForDescriptionHtmlAsync(string pageContents);
    }
}
