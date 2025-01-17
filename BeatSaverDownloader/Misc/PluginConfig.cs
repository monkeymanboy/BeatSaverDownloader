﻿using System.IO;

namespace BeatSaverDownloader.Misc
{
    public class PluginConfig
    {
        static private BS_Utils.Utilities.Config config = new BS_Utils.Utilities.Config("BeatSaverDownloader");

        public static string beatsaverURL = "https://beatsaver.com";

        public static string scoresaberURL = "https://scoresaber.com";

        public static int maxSimultaneousDownloads = 3;

        public static void LoadConfig()
        {
            if (!Directory.Exists("UserData"))
            {
                Directory.CreateDirectory("UserData");
            }

            Load();
        }

        public static void Load()
        {
            beatsaverURL = config.GetString("BeatSaverDownloader", "beatsaverURL", "https://beatsaver.com", true);
            scoresaberURL = config.GetString("BeatSaverDownloader", "scoresaberURL", "https://scoresaber.com", true);
            maxSimultaneousDownloads = config.GetInt("BeatSaverDownloader", "maxSimultaneousDownloads", 3, true);
        }

        public static void SaveConfig()
        {
            config.SetString("BeatSaverDownloader", "beatsaverURL", beatsaverURL);
            config.SetString("BeatSaverDownloader", "scoresaberURL", scoresaberURL);
            config.SetInt("BeatSaverDownloader", "maxSimultaneousDownloads", maxSimultaneousDownloads);
        }
    }
}