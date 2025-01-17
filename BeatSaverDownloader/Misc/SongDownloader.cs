﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaverDownloader.Misc
{
    public class SongDownloader : MonoBehaviour
    {
        public event Action<BeatSaverSharp.Beatmap> songDownloaded;

        private static SongDownloader _instance = null;

        public static SongDownloader Instance
        {
            get
            {
                if (!_instance)
                    _instance = new GameObject("SongDownloader").AddComponent<SongDownloader>();
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        private HashSet<string> _alreadyDownloadedSongs;
        private static bool _extractingZip;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (!SongCore.Loader.AreSongsLoaded)
            {
                SongCore.Loader.SongsLoadedEvent += SongLoader_SongsLoadedEvent;
            }
            else
            {
                SongLoader_SongsLoadedEvent(null, SongCore.Loader.CustomLevels);
            }
        }

        private void SongLoader_SongsLoadedEvent(SongCore.Loader sender, Dictionary<string, CustomPreviewBeatmapLevel> levels)
        {
            _alreadyDownloadedSongs = new HashSet<string>(levels.Values.Select(x => x.levelID.Split('_')[2]?.ToUpper()));
        }

        public async Task DownloadSong(BeatSaverSharp.Beatmap song, System.Threading.CancellationToken token, IProgress<double> progress = null, bool direct = false)
        {
            try
            {
                string customSongsPath = CustomLevelPathHelper.customLevelsDirectoryPath;
                if (!Directory.Exists(customSongsPath))
                {
                    Directory.CreateDirectory(customSongsPath);
                }
                var zip = await song.DownloadZip(direct, token, progress).ConfigureAwait(false); 
                Plugin.log.Info("Downloaded zip!");
                await ExtractZipAsync(song, zip, customSongsPath).ConfigureAwait(false);
                songDownloaded?.Invoke(song);
            }
            catch (Exception e)
            {
                if (e is TaskCanceledException)
                    Plugin.log.Warn("Song Download Aborted.");
                else
                    Plugin.log.Critical("Failed to download Song!\n" + e);
                if (_alreadyDownloadedSongs.Contains(song.Hash.ToUpper()))
                    _alreadyDownloadedSongs.Remove(song.Hash.ToUpper());
            }
        }

        private async Task ExtractZipAsync(BeatSaverSharp.Beatmap songInfo, byte[] zip, string customSongsPath, bool overwrite = false)
        {
            Stream zipStream = new MemoryStream(zip);
            try
            {
                Plugin.log.Info("Extracting...");
                _extractingZip = true;
                ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
                string basePath = songInfo.Key + " (" + songInfo.Metadata.SongName + " - " + songInfo.Metadata.LevelAuthorName + ")";
                basePath = string.Join("", basePath.Split((Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).ToArray())));
                string path = customSongsPath + "/" + basePath;

                if (!overwrite && Directory.Exists(path))
                {
                    int pathNum = 1;
                    while (Directory.Exists(path + $" ({pathNum})")) ++pathNum;
                    path += $" ({pathNum})";
                }
                Plugin.log.Info(path);
                await Task.Run(() =>
                {
                    archive.ExtractToDirectory(path);
                    foreach (var entry in archive.Entries)
                    {
                        var entryPath = Path.Combine(path, entry.Name); // Name instead of FullName for better security and because song zips don't have nested directories anyway
                        if (overwrite || !File.Exists(entryPath)) // Either we're overwriting or there's no existing file
                            entry.ExtractToFile(entryPath, overwrite);
                    }
                }).ConfigureAwait(false);
                archive.Dispose();
            }
            catch (Exception e)
            {
                Plugin.log.Critical($"Unable to extract ZIP! Exception: {e}");
                _extractingZip = false;
                return;
            }
            zipStream.Close();
        }

        public void QueuedDownload(string hash)
        {
            if (!Instance._alreadyDownloadedSongs.Contains(hash))
                Instance._alreadyDownloadedSongs.Add(hash);
        }

        public bool IsSongDownloaded(string hash)
        {
            return Instance._alreadyDownloadedSongs.Contains(hash.ToUpper());
        }
    }
}