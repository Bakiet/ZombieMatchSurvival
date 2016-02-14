using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;


public class ServiceAssistant : MonoBehaviour {
    
    bool rate_it_showed = false;

    void Start() {
        Application.runInBackground = !Application.isMobilePlatform;
        UIAssistant.onShowPage += LevelMapPopup;
        rate_it_showed = PlayerPrefs.GetInt("Rated") == 1;
    }

    void LevelMapPopup(string page) {
        StartCoroutine(LevelMapPopupRoutine(page));
    }

    IEnumerator LevelMapPopupRoutine(string page) {
        if (page != "LevelList")
            yield break;
        while (CPanel.uiAnimation > 0)
            yield return 0;
        if (UIAssistant.main.GetCurrentPage() != page)
            yield break;

        // Rate It
        if (!rate_it_showed) {
            if (ProfileAssistant.main.local_profile.current_level < 10)
                yield break;
            if (Random.value > 0.3f)
                yield break;
            yield return 0;
            UIAssistant.main.ShowPage("RateIt");
            yield break;
        }
    }

    public void RateIt() {
        string link = "";
        switch (Application.platform) {
            case RuntimePlatform.Android:
                link = "market://details?id=" + Application.bundleIdentifier;
                break;
            case RuntimePlatform.IPhonePlayer:
                link = "itms-apps://itunes.apple.com/app/id" + 1;
                break;

        }
        if (link != "")
            Application.OpenURL(link);
        rate_it_showed = true;
        PlayerPrefs.SetInt("Rated", 1);
        UIAssistant.main.SetPanelVisible("RateIt", false);
    }

    public void DownloadUpdate() {
        string link = GetAppLink();
        if (link != "")
            Application.OpenURL(link);
        UIAssistant.main.SetPanelVisible("NewVersion", false);
    }

    public void Later() {
        rate_it_showed = true;
        UIAssistant.main.SetPanelVisible("RateIt", false);
    }

    public void NoThanks() {
        rate_it_showed = true;
        PlayerPrefs.SetInt("Rated", 1);
        UIAssistant.main.SetPanelVisible("RateIt", false);
    }

    public static string GetAppLink() {
        return GetAppLink(Application.platform);
    }
    public static string GetAppLink(RuntimePlatform platform) {
        switch (platform) {
            case RuntimePlatform.WindowsEditor:
                return "https://play.google.com/store/apps/details?id=" + Application.bundleIdentifier;
            case RuntimePlatform.Android:
                return "market://details?id=" + Application.bundleIdentifier;
            case RuntimePlatform.IPhonePlayer:
                return "itms-apps://itunes.apple.com/app/id" + 1;
            default:
                return "https://www.facebook.com/games/kandilandkingdom/";
        }
    }
	
}
