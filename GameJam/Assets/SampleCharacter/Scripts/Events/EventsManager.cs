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
    [NonSerialized] public List<LongEvent> Events = new();
    [NonSerialized] public CurrentGamePhase currentGamePhase;
    public List<ShortEvent> ShortEvents;
    public List<LongEvent> LongEvents;
    public EndEvent EndEvent;
    public SpriteRenderer Father;
    public SpriteRenderer Mother;
    private Sprite FatherBaseSprite, MotherBaseSprite;
    private LongEvent CurrentEvent;
    private float eventTimer;
    public Camera Camera;
    private bool isMoving, pressedInput;
    public float vel;
    public AudioSource audioSource;
    private int currentSubEvent = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Events.Clear();
        AddShorts();
        AddUniqueLong();
        AddShorts();
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
            currentSubEvent = 0;
            StartNewEventSection(CurrentEvent.GetCurrentData(0));
            if (CurrentEvent is LongEvent LongEvent && LongEvent.Deacceleration != 0)
            {
                if (LongEvent.Deacceleration == -1)
                {
                    ChunksManager.Instance.Acceleration = 0;
                    ChunksManager.Instance.Speed = 0;
                }
                else
                {
                    ChunksManager.Instance.Acceleration = -LongEvent.Deacceleration;
                }
            }
        }
    }

    private void StartNewEventSection(EventData data)
    {
        isMoving = true;
        Textos.Instance.OpenEvent(data.Dialogue);
        changeSprites(data);
        if (data.audioClip != null)
        {
            audioSource.clip = data.audioClip;
            audioSource.Play();
        }
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
    }

    void Update()
    {

        if (CurrentEvent != null)
        {
            EventData currentData = CurrentEvent.GetCurrentData(currentSubEvent);
            if (isMoving)
            {
                if (currentData.TargetCameraPosition == TargetPosition.FadeIn || currentData.TargetCameraPosition == TargetPosition.FadeOut)
                {
                    isMoving = false;
                    eventTimer = currentData.EventDuration;
                    pressedInput = false;
                    if (currentData.TargetCameraPosition == TargetPosition.FadeIn)
                    {
                        FadeToBlack.Instance.FadeIn();
                    }
                    else
                    {
                        FadeToBlack.Instance.FadeOut();
                    }
                }
                else
                {
                    if (GoToEvent(currentData.TargetCameraPosition))
                    {
                        isMoving = false;
                        eventTimer = currentData.EventDuration;
                        pressedInput = false;
                    }
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
                        currentSubEvent += 1;
                        EventData data = longEvent.GetCurrentData(currentSubEvent);
                        if (data != null)
                        {
                            StartNewEventSection(data);
                        }
                        else
                        {
                            if (!longEvent.IsShort && currentGamePhase == CurrentGamePhase.Second) currentGamePhase = CurrentGamePhase.Third;
                            if (CurrentEvent is EndEvent)
                            {
                                SceneManager.LoadScene("GAME_CREDITS");
                            }
                            else
                            {
                                currentGamePhase = CurrentGamePhase.Second;
                                ChunksManager.Instance.Speed = ChunksManager.Instance.baseSpeed;
                                ChunksManager.Instance.Acceleration = ChunksManager.Instance.baseAcceleration;
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
                Father.enabled = true;
            }
            else
            {
                Mother.sprite = Event.SpriteTargets[i].spriteTarget;
                Mother.enabled = true;
            }
        }
    }

    void AddShorts()
    {
        ShortEvent selected = ShortEvents[UnityEngine.Random.Range(0, ShortEvents.Count)];
        Events.Add(selected);
        selected.IsShort = true;
        if (selected.shouldBeUnique) ShortEvents.Remove(selected);
    }

    void AddUniqueLong()
    {
        Events.Add(LongEvents[0]);
        LongEvents.RemoveAt(0);
    }
}