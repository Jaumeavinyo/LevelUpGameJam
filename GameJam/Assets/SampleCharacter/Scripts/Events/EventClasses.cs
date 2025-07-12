using System;
using System.Collections.Generic;
using UnityEngine;

public enum TargetPosition
{
    LeftWindow,
    CarCenter,
    RightWindow
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


}

[Serializable]
public abstract class BaseEvent
{
    public abstract EventData GetCurrentData();
}

[Serializable]
public class LongEvent : BaseEvent
{
    public List<EventData> Events;
    [Tooltip("Automatically go to play mode after event is finished")]
    public bool GoToPlayAfterEvent;
    [Tooltip("Ignore Event Duration and wait till player input")]
    public bool WaitForPlayerInput = true;

    public override EventData GetCurrentData()
    {
        return Events.Count > 0 ? Events[0] : null;
    }
}

[Serializable]
public class GasEvent : LongEvent
{
    [Tooltip("How fast the background stops moving. Leave at -1 to instantly stop!")]
    public float Deacceleration = -1;
}

[Serializable]
public class EndEvent : LongEvent
{

}