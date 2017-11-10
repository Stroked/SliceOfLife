using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

using Rareform.Validation;

namespace SoL.Core.Model
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
    public class Song
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

    [DebuggerDisplay("{Artist}-{Album}-{Title}")]
    public abstract class EsperaSong : IEquatable<EsperaSong>, INotifyPropertyChanged
    {
        /// Returns true if song is corrupted and can't be played.
        private bool _isCorrupted { get; set; }

        public bool IsCorrupted
        {
            get => this._isCorrupted;
            set
            {
                if (this._isCorrupted == value)
                {
                    return;
                }

                this._isCorrupted = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public Guid Guid { get; }
        public string Album { get; internal set; }
        public string Artist { get; internal set; }
        public TimeSpan Duration { get; private set; }
        public string Genre { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Song" /> class.
        /// </summary>
        /// <param name="path">The local file directory path to the song.</param>
        /// <param name="duration">The duration of the song.</param>
        /// <exception cref="ArgumentNullException"><c>path</c> is null.</exception>
        protected EsperaSong(string path, TimeSpan duration)
        {
            this.OriginalPath = path ?? throw new ArgumentNullException(nameof(path));
            this.Duration = duration;

            this.Album = string.Empty;
            this.Artist = string.Empty;
            this.Genre = string.Empty;
            this.Title = string.Empty;
            this.Guid = Guid.NewGuid();
        }

        /// <summary>
        /// Gets the path of the song on the local filesystem, or in the internet.
        /// </summary>
        public string OriginalPath { get; }

        /// <summary>
        /// Gets the path to stream the audio from.
        /// </summary>
        public abstract string PlaybackPath { get; }

        public string Title { get; set; }

        public int TrackNumber { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as EsperaSong);
        }

        public bool Equals(EsperaSong other)
        {
            return other != null && this.OriginalPath == other.OriginalPath;
        }

        public override int GetHashCode() => this.OriginalPath.GetHashCode();

        public override string ToString() => $"Title: {this.Title}, Artist: {this.Artist}, Path: {this.OriginalPath}";

        public bool UpdateMetadataFrom(EsperaSong song)
        {
            if (song == null)
            {
                Throw.ArgumentNullException(() => song);
            }

            if (this.OriginalPath != song?.OriginalPath)
            {
                Throw.ArgumentException("The original path of both songs must be the same", () => song);
            }

            // NB: Wow this is dumb
            bool changed = false;

            if (this.Album != song.Album)
            {
                this.Album = song.Album;
                changed = true;
            }

            if (this.Artist != song.Artist)
            {
                this.Artist = song.Artist;
                changed = true;
            }

            if (this.Duration != song.Duration)
            {
                this.Duration = song.Duration;
                changed = true;
            }

            if (this.Genre != song.Genre)
            {
                this.Genre = song.Genre;
                changed = true;
            }

            if (this.Title != song.Title)
            {
                this.Title = song.Title;
                changed = true;
            }

            if (this.TrackNumber != song.TrackNumber)
            {
                this.TrackNumber = song.TrackNumber;
                changed = true;
            }

            return changed;
        }

        ///// Prepares the song for playback.
        //internal virtual Task PrepareAsync(YoutubeStreamingQuality qualityHint)
        //{
        //    return Task.Delay(0);
        //}


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            try
            {
                handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception)
            {
                Throw.ArgumentNullException(() => "Null Delegate Exception");
            }
        }
    }
}