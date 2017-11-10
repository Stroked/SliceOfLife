using System;

namespace SoL.Core.Interface
{
    public interface ILocalSongFinder
    {
        /// <summary>
        /// This method scans the directory, specified in the constructor,and returns an observable with a tuple that contains the song and the data of the artwork.
        /// </summary>
        IObservable<(ILocalSong, byte[])> GetSongsAsync();
    }
}