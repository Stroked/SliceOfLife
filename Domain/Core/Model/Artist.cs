using System;

namespace SoL.Core.Model
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
