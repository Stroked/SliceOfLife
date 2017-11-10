using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Linq;

using Rareform.Validation;

using ReactiveUI;

using SoL.Core.Interface;

using Splat;

using TagLib;

namespace SoL.Core.Local

{
    /// <summary>
    /// Encapsulates a recursive call through the local filesystem that reads the tags of all WAV
    /// and MP3 files and returns them.
    /// </summary>
    internal sealed class LocalSongFinder : ILocalSongFinder,
        IEnableLogger
    {
        private static readonly string[] AllowedExtensions = { ".mp3", ".wav", ".m4a", ".aac" };
        private readonly string directoryPath;
        private readonly IFileSystem fileSystem;

        public LocalSongFinder(string directoryPath, IFileSystem fileSystem = null)
        {
            if (directoryPath == null)
            {
                Throw.ArgumentNullException(() => directoryPath);
            }

            this.directoryPath = directoryPath;
            this.fileSystem = fileSystem ?? new FileSystem();
        }

        /// <summary>
        /// This method scans the directory, specified in the constructor, and returns an observable with a tuple that contains the song and the data of the artwork.
        /// </summary>
        public IObservable<(ILocalSong, byte[])> GetSongsAsync()
        {
            return ScanDirectoryForValidPaths(directoryPath)
                .Select(ProcessFile)
                .Where(t => (t.Item1 != null) && (t.Item2 != null))
                .ToObservable(RxApp.TaskpoolScheduler);
        }

        private static (ILocalSong, byte[]) CreateSong(Tag tag, TimeSpan duration, string filePath)
        {
            var song = new LocalSong(filePath, duration)
            {
                Album = PrepareTag(tag.Album, string.Empty),
                Artist =
                    PrepareTag(tag.FirstAlbumArtist ?? tag.FirstPerformer,
                        "Unknown Artist"), //HACK: In the future retrieve the string for an unkown artist from the view if we want to localize it
                Genre = PrepareTag(tag.FirstGenre, string.Empty),
                Title = PrepareTag(tag.Title, Path.GetFileNameWithoutExtension(filePath)),
                TrackNumber = (int)tag.Track
            };

            IPicture picture = tag.Pictures.FirstOrDefault();

            return (song, picture?.Data.Data);
        }

        private static string PrepareTag(string tag, string replacementIfNull)
        {
            return tag == null ? replacementIfNull : Utility.TagCleaner(tag);
        }

        private (ILocalSong, byte[]) ProcessFile(string filePath)
        {
            try
            {
                using (var fileAbstraction = new TagLibFileAbstraction(filePath, fileSystem))
                {
                    using (var file = TagLib.File.Create(fileAbstraction))
                    {
                        return CreateSong(file.Tag, file.Properties.Duration, file.Name);
                    }
                }
            }

            catch (Exception ex)
            {
                this.Log().ErrorException($"Couldn't read song file {filePath}", ex);
                return (null, null);
            }
        }

        private IEnumerable<string> ScanDirectoryForValidPaths(string rootPath)
        {
            IEnumerable<string> files = Enumerable.Empty<string>();

            try
            {
                files = fileSystem.Directory.GetFiles(rootPath)
                    .Where(x => AllowedExtensions.Contains(Path.GetExtension(x)?.ToLowerInvariant()));
            }

            catch (Exception ex)
            {
                this.Log().ErrorException($"Couldn't get files from directory {rootPath}", ex);
            }

            IEnumerable<string> directories = Enumerable.Empty<string>();

            try
            {
                directories = fileSystem.Directory.GetDirectories(rootPath);
            }

            catch (Exception ex)
            {
                this.Log().ErrorException($"Couldn't get directories from directory {rootPath}", ex);
            }

            return files.Concat(directories.SelectMany(ScanDirectoryForValidPaths));
        }

        private class TagLibFileAbstraction : TagLib.File.IFileAbstraction, IDisposable
        {
            public TagLibFileAbstraction(string path, IFileSystem fileSystem)
            {
                if (fileSystem == null)
                {
                    throw new ArgumentNullException(nameof(fileSystem));
                }

                Name = path ?? throw new ArgumentNullException(nameof(path));

                Stream stream = fileSystem.File.OpenRead(path);

                ReadStream = stream;
                WriteStream = stream;
            }

            public string Name { get; }

            public Stream ReadStream { get; }

            public Stream WriteStream { get; }

            public void CloseStream(Stream stream)
            {
                stream.Close();
            }

            public void Dispose()
            {
                ReadStream.Dispose();
            }
        }
    }
}