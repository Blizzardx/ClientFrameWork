using UnityEngine;
using System;
using System.Collections;

public enum AudioId
{
    None,

    //background
    LogIn,
    Menu,
    RoomList,
    WaitForBattle,
    Battle_0,
    Battle_1,
    Battle_2,
    Battle_3,
    Battle_4,
    BattleVictory,
    BattleFailed,
    Normal,

    //effect
    Fire,
    Explosion,
    Lightning,
    Pulse,
    Ray,
    Reload,

    //ui
    OnClick,
}

public class AudioDefiner
{
    public static void RegisterAudio()
    {
        AudioManager.Instance.RegisterAudio(AudioId.LogIn, new AudioIndexStruct("music_defeat"));
        AudioManager.Instance.RegisterAudio(AudioId.Battle_0, new AudioIndexStruct("music_level_a"));
    }
}