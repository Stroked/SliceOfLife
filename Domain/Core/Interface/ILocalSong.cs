using System.Threading.Tasks;

namespace SoL.Core.Interface
{
    public interface ILocalSong
    {
        string ArtworkKey { get; }
        string PlaybackPath { get; }
        Task SaveTagsToDisk();

    }
}
