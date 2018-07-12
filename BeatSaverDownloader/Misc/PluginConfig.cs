﻿using IllusionPlugin;
using SongBrowserPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BeatSaverDownloader
{
    class PluginConfig
    {
        static private string configPath = "favoriteSongs.cfg";
        static private string songBrowserSettings = "song_browser_settings.xml";

        public static List<string> favoriteSongs = new List<string>();

        public static string beatsaverURL = "https://beatsaver.com";
        public static string apiAccessToken { get; private set; }

        public static string apiTokenPlaceholder = "replace-this-with-your-api-token";

        public static void LoadOrCreateConfig()
        {
            if (!Directory.Exists("UserData"))
            {
                Directory.CreateDirectory("UserData");
            }

            if (!ModPrefs.HasKey("BeatSaverDownloader", "beatsaverURL"))
            {
                ModPrefs.SetString("BeatSaverDownloader", "beatsaverURL", "https://beatsaver.com");
                Logger.StaticLog("Created config");
            }
            else
            {
                beatsaverURL = ModPrefs.GetString("BeatSaverDownloader", "beatsaverURL");
                if (string.IsNullOrEmpty(beatsaverURL))
                {
                    ModPrefs.SetString("BeatSaverDownloader", "beatsaverURL", "https://beatsaver.com");
                    beatsaverURL = "https://beatsaver.com";
                    Logger.StaticLog("Created config");
                }
                else
                {
                    Logger.StaticLog("Loaded config");
                }
            }
            
            if (string.IsNullOrEmpty(ModPrefs.GetString("BeatSaverDownloader", "apiAccessToken")))
            {
                ModPrefs.SetString("BeatSaverDownloader", "apiAccessToken", apiTokenPlaceholder);
            }
            else
            {
                apiAccessToken = ModPrefs.GetString("BeatSaverDownloader", "apiAccessToken");
                if (string.IsNullOrEmpty(beatsaverURL))
                {
                    ModPrefs.SetString("BeatSaverDownloader", "apiAccessToken", apiTokenPlaceholder);
                    apiAccessToken = apiTokenPlaceholder;
                }
            }

            if (!File.Exists(configPath))
            {
                File.Create(configPath);
            }

            favoriteSongs.AddRange(File.ReadAllLines(configPath, Encoding.UTF8));
            

            if (!File.Exists(songBrowserSettings))
            {
                return;
            }
        
            FileStream fs = null;
            try
            {
                fs = File.OpenRead(songBrowserSettings);

                XmlSerializer serializer = new XmlSerializer(typeof(SongBrowserSettings));

                SongBrowserSettings settings = (SongBrowserSettings)serializer.Deserialize(fs);

                favoriteSongs.AddRange(settings.favorites);

                fs.Close();
                
                SaveConfig();
            }
            catch (Exception e)
            {
                Logger.StaticLog($"Can't parse BeatSaberSongBrowser settings file! Exception: {e}");
                if (fs != null) { fs.Close(); }
            }
        }
        
        public static void SaveConfig()
        {
            File.WriteAllLines(configPath, favoriteSongs.Distinct().ToArray(), Encoding.UTF8);
        }

    }
}
