using System;

namespace SliceOfLife.Console.Core
{
    public class Artist
    {
        public Artist(string name)
        {
            ArtistId = new Guid();
            Name = name;
        }

        public readonly Guid ArtistId;
        public readonly string Name;
    }
}
