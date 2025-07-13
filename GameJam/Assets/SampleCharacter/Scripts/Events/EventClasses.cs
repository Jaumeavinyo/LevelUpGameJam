using System;
using System.Collections.Generic;
using UnityEngine;

public enum TargetPosition
{
    LeftWindow,
    CarCenter,
    RightWindow,
    FadeIn,
    FadeOut
}
[Serializable]
public struct SpriteTarget
{
    [Tooltip("Select if we want to change the mother or the father sprite")]
    public TargetName target;//father or mother
    [Tooltip("Select the sprite we want to use to substitute father or mother selected in target")]
    public Sprite spriteTarget;//The sprite we want to put on screen instead of fathermother base
}

[Serializable]
public class EventData
{
    [Tooltip("The camera position to move to at this time")]
    public TargetPosition TargetCameraPosition;
    [Tooltip("Sprites to change in this event")]
    public List<SpriteTarget> SpriteTargets;
    [Tooltip("Text to display in this event")]
    public string Dialogue;
    [Tooltip("Event duration")]
    public float EventDuration;
    [Tooltip("Music to play in this event, if none is selected, current playing music will be used in fade outs if wanted")]
    public MusicTheme musicTheme;
    [Tooltip("currentMusic will fade out to the desired volume")]
    public float MusicVolumeDuringEvent;
    [Tooltip("currentMusic will fade out in a time duration")]
    public float fadeDuration;

    public AudioClip audioClip;
}

[Serializable]
public class LongEvent
{
    public List<EventData> Events;
    [Tooltip("Automatically go to play mode after event is finished")]
    public bool GoToPlayAfterEvent;
    [Tooltip("Ignore Event Duration and wait till player input")]
    public bool WaitForPlayerInput = true;
    [NonSerialized] public bool IsShort;
    [Tooltip("How fast the background stops moving. Leave at -1 to instantly stop!")]
    public float Deacceleration = 0;

    public EventData GetCurrentData(int index)
    {
        return Events.Count > index ? Events[index] : null;
    }
}

[Serializable]
public class ShortEvent : LongEvent
{
    public bool shouldBeUnique;
}

[Serializable]
public class EndEvent : LongEvent
{

}
