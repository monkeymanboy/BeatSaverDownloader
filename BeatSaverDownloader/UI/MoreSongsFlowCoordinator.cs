﻿using BeatSaberMarkupLanguage;
using BeatSaverDownloader.UI.ViewControllers;
using HMUI;
using System;
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

        public void Awake()
        {
            if (_moreSongsView == null)
            {
                _moreSongsView = BeatSaberUI.CreateViewController<MoreSongsListViewController>();
                _songDetailView = BeatSaberUI.CreateViewController<SongDetailViewController>();
                _moreSongsNavigationcontroller = BeatSaberUI.CreateViewController<NavigationController>();
                _moreSongsView.navController = _moreSongsNavigationcontroller;
                _songDescriptionView = BeatSaberUI.CreateViewController<SongDescriptionViewController>();
                _downloadQueueView = BeatSaberUI.CreateViewController<DownloadQueueViewController>();

                _moreSongsView.didSelectSong += HandleDidSelectSong;
                _moreSongsView.filterDidChange += HandleFilterDidChange;
                _songDetailView.didPressDownload += HandleDidPressDownload;
                _songDetailView.didPressUploader += HandleDidPressUploader;
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

        internal void HandleDidSelectSong(BeatSaverSharp.Beatmap song, Texture2D cover = null)
        {
            _songDetailView.ClearData();
            _songDescriptionView.ClearData();
            if (!_songDetailView.isInViewControllerHierarchy)
            {
                PushViewControllerToNavigationController(_moreSongsNavigationcontroller, _songDetailView);
            }
            SetRightScreenViewController(_songDescriptionView);
            _songDescriptionView.Initialize(song);
            _songDetailView.Initialize(song, cover);
        }

        internal void HandleDidPressDownload(BeatSaverSharp.Beatmap song, Texture2D cover)
        {
            Plugin.log.Info("Download pressed for song: " + song.Metadata.SongName);
            //    Misc.SongDownloader.Instance.DownloadSong(song);
            Misc.SongDownloader.Instance.QueuedDownload(song.Hash.ToUpper());
            _songDetailView.UpdateDownloadButtonStatus();
            _downloadQueueView.EnqueueSong(song, cover);
        }
        internal void HandleDidPressUploader(BeatSaverSharp.User uploader)
        {
            Plugin.log.Info("Uploader pressed for user: " + uploader.Username);
            _moreSongsView.SortByUser(uploader);
        }
        internal void HandleFilterDidChange()
        {
            if (_songDetailView.isInViewControllerHierarchy)
            {
                PopViewControllersFromNavigationController(_moreSongsNavigationcontroller, 1);
            }
            _songDetailView.ClearData();
            _songDescriptionView.ClearData();
        }
        protected override void BackButtonWasPressed(ViewController topViewController)
        {

            _moreSongsView.Cleanup();
            var mainFlow = BeatSaberMarkupLanguage.BeatSaberUI.MainFlowCoordinator;
            mainFlow.DismissFlowCoordinator(this);
        }
    }
}