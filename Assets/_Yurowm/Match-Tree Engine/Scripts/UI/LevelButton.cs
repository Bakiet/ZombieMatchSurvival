﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LevelButton : MonoBehaviour {

    public int level = 0;
    //public Button button;
    public ScoreStarFromMemory stars;
    public TextMesh level_number;
    public SpriteRenderer background;
    public FieldTarget[] targets;
    public Sprite locked_sprite;
    public Sprite[] sprites;

    Animation anim;

    void Awake() {
        anim = GetComponent<Animation>();
    }


    public void Initialize() {
        stars.level = level;
        stars.Resresh();
        level_number.text = level.ToString();

        transform.FindChild("BG").localScale = Vector3.one;
        if (ProfileAssistant.main.local_profile.current_level == level)
            anim.Play();
        else
            anim.Stop();


        if (IsLocked())
            Lock();
        else
            Unlock();
    }

    bool IsLocked() {
        return ProfileAssistant.main.local_profile.current_level < level;
    }

    void Unlock() {
        background.sprite = sprites[Array.IndexOf(targets, Level.all[level].target)];
        stars.gameObject.SetActive(true);
    }

    void Lock() {
        background.sprite = locked_sprite;
        stars.gameObject.SetActive(false);
    }

    public void OnClick() {
        MapLocation location = GetComponentInParent<MapLocation>();

        if (location)
            location.ApplyBackground();

        if (!IsLocked())
            Level.LoadLevel(level);
    }
}
