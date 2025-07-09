using System;
using System.Collections.Generic;
using UnityEngine;


public class EventsManager : MonoBehaviour
{
    [NonSerialized] public List<StructEvent> Events = new();
    public List<StructEvent> ShortEvents, LongEvents;
    public StructEvent GasEvent, EndEvent;
    HashSet<StructEvent> usedLongEvents = new();

    public SpriteRenderer Father;
    public SpriteRenderer Mother;


    void Start()
    {
        AddShorts();
        AddUniqueLong();
        AddShorts();
        Events.Add(GasEvent);
        AddShorts();
        AddUniqueLong();
        AddShorts();
        Events.Add(EndEvent);

       
    }

    public void changeSprites(StructEvent Event)
    {
        for(int i = 0; i < Event.SpriteTargets.Count; i++)
        {
            if(Event.SpriteTargets[i].target == TargetName.FATHER){//cambiar el sprite base del padre por otro del padre
                Father.sprite = Event.SpriteTargets[i].spriteTarget;
            }
            else
            {
                Mother.sprite = Event.SpriteTargets[i].spriteTarget;
            }
        }
    }

    void AddShorts()
    {
        List<StructEvent> pool = new(ShortEvents);
        for (int i = 0; i < 2; i++)
        {
            if (pool.Count == 0) pool = new(ShortEvents);
            var selected = pool[UnityEngine.Random.Range(0, pool.Count)];
            selected.IsShortEvent = true;
            Events.Add(selected);
            pool.Remove(selected);
        }
    }

    void AddUniqueLong()
    {
        List<StructEvent> availableLongs = LongEvents.FindAll(e => !usedLongEvents.Contains(e));
        if (availableLongs.Count == 0) return;

        var selected = availableLongs[UnityEngine.Random.Range(0, availableLongs.Count)];
        Events.Add(selected);
        usedLongEvents.Add(selected);
    }
}