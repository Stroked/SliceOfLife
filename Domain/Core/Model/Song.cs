using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using SliceOfLife.Console.Core;

namespace SliceOfLife.Console
{
    /*
            ********* There are 3 types of Song *****
   1. SoloSong - This is a single artist song. 
        (namingFormat: ArtistName - SongName)
            eg. "The Strokes - Is This It"
   2. FeaturingArtistSong - Second most common type of song. 
        BizRule: There can be multiple featuring artists on a single song and it is treated the same way as a single featuring artist. 
        (namingFormat: ArtistName - SongName Ft. FeaturingArtistName) 
            eg. B.o.B - Airplanes, Part II Ft. Eminem
    3. CollaborationSong. This is the least common type of song.
        BizRule: A lot of collaboration songs will be treated as featuringArtistSongs because I want them to show up under a specific artists playlist. For Example Eminem and Nate Dogg collaborate on 'Till I Collapse but the song will be classified as a FeaturingArtist with Eminem as the main act and Nate Dogg as the Featuring Artist.
        (namingFormat: Artist1 & Artist2 - SongName)
            eg. "Norah Jones & Billy Joe Armstrong - Long Time Gone" 
            
   
    */
    class Song
    {
        public Song(Artist primaryArtist, IEnumerable<Artist> featureArtists, string title, string album, Genre genre) : this(primaryArtist, title, album, genre)
        {
            FeatureArtist = featureArtists.Aggregate("", (x, y) => $"{x} & {y}");
        }

        public Song(Artist artist, string title, string album, Genre genre)
        {
            Title = title;
            Artist = artist.ToString();
            Album = album;
            Genre = genre;
        }

        public readonly string FeatureArtist;
        public readonly string Title;
        public readonly string Artist;
        public readonly string Album;
        public readonly Genre Genre;

        public override string ToString()
        {
            return $"{Artist}-{Title}{(!string.IsNullOrEmpty(FeatureArtist) ? FeatureArtist : "")}";
        }
    }
}