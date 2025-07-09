using System;
using System.Collections.Generic;
using UnityEngine;


public class EventsManager : MonoBehaviour
{
    [NonSerialized] public List<StructEvent> Events = new();
    public List<StructEvent> ShortEvents, LongEvents;
    public StructEvent GasEvent, EndEvent;
    HashSet<StructEvent> usedLongEvents = new();

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