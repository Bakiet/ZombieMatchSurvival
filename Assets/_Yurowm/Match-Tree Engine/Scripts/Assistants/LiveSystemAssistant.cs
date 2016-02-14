using UnityEngine;
using System.Collections;
using System;
using Soomla.Store;

public class LiveSystemAssistant : MonoBehaviour {

    public static readonly TimeSpan refilling_time = new TimeSpan(0, 30, 0);
    public static readonly int lives_limit = 5;
	void Start () {
        StartCoroutine(LiveSystemRoutine());
	}

    IEnumerator LiveSystemRoutine() {
        while (ProfileAssistant.main.local_profile == null)
            yield return 0;

        while (true) {
            if (ProfileAssistant.main.local_profile["live"] < lives_limit) {
                if (ProfileAssistant.main.local_profile.next_live_time <= DateTime.Now) {
                    AddLive();
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    public static void BurnLive() {
        if (ProfileAssistant.main.local_profile["live"] >= lives_limit)
            ProfileAssistant.main.local_profile.next_live_time = DateTime.Now + refilling_time;
        ProfileAssistant.main.local_profile["live"]--;
    }

    public static int lives {
        get {
            return ProfileAssistant.main.local_profile["live"];
        }
        set {
            if (ProfileAssistant.main.local_profile["live"] == lives_limit && value < lives_limit)
                ProfileAssistant.main.local_profile.next_live_time = DateTime.Now + refilling_time;
            if (value >= lives_limit)
                ProfileAssistant.main.local_profile.next_live_time = new DateTime();

        }
    }

    public static void AddLive() {
        if (ProfileAssistant.main.local_profile["live"] < lives_limit) {
            ProfileAssistant.main.local_profile["live"] += Mathf.FloorToInt((float) ((DateTime.Now - ProfileAssistant.main.local_profile.next_live_time).TotalMinutes / refilling_time.TotalMinutes));
            if (ProfileAssistant.main.local_profile["live"] > lives_limit)
                ProfileAssistant.main.local_profile["live"] = lives_limit;
        }
        if (ProfileAssistant.main.local_profile["live"] == lives_limit)
            ProfileAssistant.main.local_profile.next_live_time = new DateTime();

        ItemCounter.RefreshAll();
    }


}
