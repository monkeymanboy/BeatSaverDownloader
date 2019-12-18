﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using HMUI;
using BeatSaverDownloader.UI.ViewControllers;
using BS_Utils.Utilities;
using UnityEngine;
namespace BeatSaverDownloader.UI
{
    public class MoreSongsFlowCoordinator : FlowCoordinator
    {
        private NavigationController _moreSongsNavigationcontroller;
        private MoreSongsListViewController _moreSongsView;
        private SongDetailViewController _songDetailView;
        private SongDescriptionViewController _songDescriptionView;
        private DownloadQueueViewController _downloadQueueView;

        public static Action<BeatSaverSharp.Beatmap, Texture2D> didSelectSong;
        public static Action<BeatSaverSharp.Beatmap> didPressDownload;
        public void Awake()
        {
            if (_moreSongsView == null)
            {
                _moreSongsView = BeatSaberUI.CreateViewController<MoreSongsListViewController>();
                _songDetailView = BeatSaberUI.CreateViewController<SongDetailViewController>();
                _moreSongsNavigationcontroller = BeatSaberUI.CreateViewController<NavigationController>();

                _songDescriptionView = BeatSaberUI.CreateViewController<SongDescriptionViewController>();
                _downloadQueueView = BeatSaberUI.CreateViewController<DownloadQueueViewController>();


                didSelectSong += DidSelectSong;
                didPressDownload += DidPressDownload;
            }
        }
        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            try
            {
                if (firstActivation)
                {

                    title = "More Songs";
                    showBackButton = true;

                    SetViewControllerToNavigationConctroller(_moreSongsNavigationcontroller, _moreSongsView);
                    ProvideInitialViewControllers(_moreSongsNavigationcontroller, _downloadQueueView);
                  //  PopViewControllerFromNavigationController(_moreSongsNavigationcontroller);
                    
                }
                if (activationType == ActivationType.AddedToHierarchy)
                {

                }

            }
            catch (Exception ex)
            {
                Plugin.log.Error(ex);
            }
        }

        internal void DidSelectSong(BeatSaverSharp.Beatmap song, Texture2D cover = null)
        {
            _songDetailView.ClearData();
            _songDescriptionView.ClearData();
            if(!_songDetailView.isInViewControllerHierarchy)
            {
            PushViewControllerToNavigationController(_moreSongsNavigationcontroller, _songDetailView);
            }
            SetRightScreenViewController(_songDescriptionView);
            _songDescriptionView.Initialize(song);
            _songDetailView.Initialize(song, cover);
        }

        internal void DidPressDownload(BeatSaverSharp.Beatmap song)
        {
            Plugin.log.Info("Download pressed for song: " + song.Metadata.SongName);
        }
        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            // dismiss ourselves
            var mainFlow = BeatSaberMarkupLanguage.BeatSaberUI.MainFlowCoordinator;
            mainFlow.InvokeMethod("DismissFlowCoordinator", this, null, false);
        }
    }
}
