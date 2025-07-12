using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum CurrentGamePhase
{
    First,
    Second, // luego de gas (1/2)
    Third // luego de evento largo despues de gas (3/4)
}


public class EventsManager : MonoBehaviour
{
    public SoundManager soundManager;
    public Transform CarCenter, LWindow, RWindow;
    public static EventsManager Instance;
    [NonSerialized] public List<BaseEvent> Events = new();
    [NonSerialized] public CurrentGamePhase currentGamePhase;
    public List<LongEvent> ShortEvents;
    public List<LongEvent> LongEvents;
    public GasEvent GasEvent;
    public EndEvent EndEvent;
    public SpriteRenderer Father;
    public SpriteRenderer Mother;
    private Sprite FatherBaseSprite, MotherBaseSprite;
    private BaseEvent CurrentEvent;
    private float eventTimer;
    public Camera Camera;
    private bool isMoving, pressedInput;
    public float vel;



    void Awake()
    {
        Instance = this;
    }

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

        FatherBaseSprite = Father.sprite;
        MotherBaseSprite = Mother.sprite;
        currentGamePhase = CurrentGamePhase.First;
        soundManager = FindFirstObjectByType<SoundManager>();
    }

    public void PressedInput()
    {
        pressedInput = true;
    }

    public void StartNextEvent()
    {
        if (Events.Count > 0)
        {
            CurrentEvent = Events[0];
            Events.RemoveAt(0);
            StartNewEventSection(CurrentEvent.GetCurrentData());
            if (CurrentEvent is GasEvent gasEvent)
            {
                if (gasEvent.Deacceleration == -1)
                {
                    ChunksManager.Instance.Acceleration = 0;
                    ChunksManager.Instance.Speed = 0;
                }
                else
                {
                    ChunksManager.Instance.Acceleration = -gasEvent.Deacceleration;
                }
            }
        }
    }

    private void StartNewEventSection(EventData data)
    {
        isMoving = true;
        Textos.Instance.OpenEvent(data.Dialogue);
        changeSprites(data);
        if (soundManager)
        {
            if (data.musicTheme == MusicTheme.NONE)
            {
                StartCoroutine(soundManager.MusicFadeOut(data.fadeDuration, soundManager.currentMusicSource.volume, data.MusicVolumeDuringEvent));
            }
            else if (data.musicTheme != MusicTheme.NONE && data.musicTheme != soundManager.getMusicByAudioSource(soundManager.currentMusicSource).theme)
            {
                StartCoroutine(soundManager.CrossfadeMusic(soundManager.currentMusicSource, soundManager.getMusicBytheme(data.musicTheme).music, data.fadeDuration));
            }


        }
        //if (data.Music != null)
        //{
        //    // PLAY CLIP
        //}
    }

    void Update()
    {
        if (CurrentEvent != null)
        {
            EventData currentData = CurrentEvent.GetCurrentData();
            if (isMoving)
            {
                if (GoToEvent(currentData.TargetCameraPosition))
                {
                    eventTimer = currentData.EventDuration;
                    pressedInput = false;
                }
            }
            else
            {

                eventTimer -= Time.deltaTime;
                bool waitForInput = CurrentEvent is LongEvent lE && lE.WaitForPlayerInput;
                if ((!waitForInput && eventTimer <= 0) || (waitForInput && pressedInput))
                {
                    if (CurrentEvent is LongEvent longEvent)
                    {
                        longEvent.Events.RemoveAt(0);
                        EventData data = longEvent.GetCurrentData();
                        if (data != null)
                        {
                            StartNewEventSection(data);
                        }
                        else
                        {
                            if (currentGamePhase == CurrentGamePhase.Second) currentGamePhase = CurrentGamePhase.Third;
                            if (CurrentEvent is EndEvent)
                            {
                                // END GAME
                            }
                            else
                            {
                                if (CurrentEvent is GasEvent)
                                {
                                    currentGamePhase = CurrentGamePhase.Second;
                                    ChunksManager.Instance.Speed = ChunksManager.Instance.baseSpeed;
                                    ChunksManager.Instance.Acceleration = ChunksManager.Instance.baseAcceleration;
                                }
                                FinishEvent(longEvent.GoToPlayAfterEvent);
                            }
                        }
                    }
                }
            }
        }
    }

    bool GoToEvent(TargetPosition eventTargetPosition)
    {
        Transform target = eventTargetPosition switch
        {
            TargetPosition.RightWindow => RWindow,
            TargetPosition.CarCenter => CarCenter,
            _ => LWindow
        };
        Vector3 targetPosition = new(target.position.x, target.position.y, -1);
        Camera.transform.position = Vector3.MoveTowards(Camera.transform.position, targetPosition, vel * Time.deltaTime);

        if (Vector3.Distance(Camera.transform.position, targetPosition) == 0)
        {
            isMoving = false;
            return true;
        }
        return false;
    }

    public void FinishEvent(bool sendPlayerBackToGameMode)
    {
        CurrentEvent = null;
        GameManager.Instance.FinishEvent(sendPlayerBackToGameMode);
        Father.sprite = FatherBaseSprite;
        Mother.sprite = MotherBaseSprite;
        Textos.Instance.FinishEvent();


    }

    public void changeSprites(EventData Event)
    {
        for (int i = 0; i < Event.SpriteTargets.Count; i++)
        {
            if (Event.SpriteTargets[i].target == TargetName.FATHER)
            {//cambiar el sprite base del padre por otro del padre
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
        List<LongEvent> pool = new(ShortEvents);
        for (int i = 0; i < 2; i++)
        {
            if (pool.Count == 0) pool = new(ShortEvents);
            var selected = pool[UnityEngine.Random.Range(0, pool.Count)];
            Events.Add(selected);
            pool.Remove(selected);
        }
    }

    void AddUniqueLong()
    {
        Events.Add(LongEvents[0]);
        Events.RemoveAt(0);
    }
}