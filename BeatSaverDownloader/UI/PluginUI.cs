﻿using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using HMUI;
using System.Linq;
using UnityEngine;

namespace BeatSaverDownloader.UI
{
    public class PluginUI : PersistentSingleton<PluginUI>
    {
        public MenuButton moreSongsButton;
        internal MoreSongsFlowCoordinator _moreSongsFlowCooridinator;
        public static GameObject _levelDetailClone;

        internal void Setup()
        {
            moreSongsButton = new MenuButton("More Songs", "Download More Songs from here!", MoreSongsButtonPressed, false);
            MenuButtons.instance.RegisterButton(moreSongsButton);
        }

        internal static void SetupLevelDetailClone()
        {
            _levelDetailClone = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<StandardLevelDetailView>().First(x => x.name == "LevelDetail").gameObject);
            _levelDetailClone.gameObject.SetActive(false);
            Destroy(_levelDetailClone.GetComponent<StandardLevelDetailView>());
            var bsmlObjects = _levelDetailClone.GetComponentsInChildren<RectTransform>().Where(x => x.gameObject.name.StartsWith("BSML"));
            var hoverhints = _levelDetailClone.GetComponentsInChildren<HoverHint>();
            var localHoverHints = _levelDetailClone.GetComponentsInChildren<LocalizedHoverHint>();
            foreach (var bsmlObject in bsmlObjects)
                Destroy(bsmlObject.gameObject);
            foreach (var hoverhint in hoverhints)
                Destroy(hoverhint);
            foreach (var hoverhint in localHoverHints)
                Destroy(hoverhint);
            Destroy(_levelDetailClone.transform.Find("Level").Find("FavoritesToggle").gameObject);
            Destroy(_levelDetailClone.transform.Find("PlayContainer").Find("PlayButtons").gameObject);
            Destroy(_levelDetailClone.transform.Find("Stats").Find("MaxCombo").gameObject);
            Destroy(_levelDetailClone.transform.Find("Stats").Find("Highscore").gameObject);
            Destroy(_levelDetailClone.transform.Find("Stats").Find("MaxRank").gameObject);
        }

        internal void MoreSongsButtonPressed()
        {
            ShowMoreSongsFlow();
        }

        internal void ShowMoreSongsFlow()
        {
            if (_moreSongsFlowCooridinator == null)
                _moreSongsFlowCooridinator = BeatSaberUI.CreateFlowCoordinator<MoreSongsFlowCoordinator>();
            BeatSaberMarkupLanguage.BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_moreSongsFlowCooridinator); // ("PresentFlowCoordinator", _moreSongsFlowCooridinator, null, false, false);
        }
    }
}