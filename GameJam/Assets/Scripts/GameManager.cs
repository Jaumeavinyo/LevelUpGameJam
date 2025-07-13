using UnityEngine;
using System;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.UI;
using System.Collections.Generic;


public enum TargetName
{
    FATHER,
    MOTHER,
}

public enum GamePlayMode
{
    PLAYING,
    IN_EVENT,
    FREE_MOVEMENT
}

public class GameManager : MonoBehaviour
{
    //INPUT SYSTEM
    public InputActionAsset inputActions;
    public InputAction inputAction_move_camera, inputAction_interact;
    public static string LastInputDevice = "None";
    public PlayerInput playerInput;
    public InputDeviceDetector inputdetector;

    public float secondsToForceEventIfNointeraction;
    [NonSerialized] public string playerName;

    SoundManager soundManager;

    public static GameManager Instance;
    public TextMeshProUGUI StateText;
    public Vector3 CAMERA_POSITION_FOR_GAME = new(-16.5f, 1, -1);
    public Camera Camera;
    public static float IN_GAME_CAMERA_SIZE = 4.75f, IN_EVENT_CAMERA_SIZE = 6;
    public static float TIME_BETWEEN_EVENTS = 5;
    public float freeMovementVel;
    private float nextEventTimer = 0, forceEventTimer = 0;
    [NonSerialized] public GamePlayMode gamePlayMode;
    public GameObject PressEGameObject, PressAgameObject;

    public float minX = -18, maxX = 18, minY = -1, maxY = 1;
    private bool ChangingToPlayMode = false;

    void Start()
    {
        gamePlayMode = GamePlayMode.FREE_MOVEMENT;
        playerName = PlayerData.playerName;
        Debug.Log(playerName);
        SoundManager.Instance.PlayMusic(MusicTheme.GAME_START);
        soundManager = FindFirstObjectByType<SoundManager>();
        soundManager.changeAllMusicVolume(soundManager.GameMusicList[1].maxVolume);//esto es una puta chapuza q soluciona un bug de reseteo de volumen de los sourceSounds
    }

    void Awake()
    {
        Instance = this;
    }

    public void FinishEvent(bool backToPlay)
    {
        ChangingToPlayMode = backToPlay;
        gamePlayMode = GamePlayMode.FREE_MOVEMENT;
    }

    void Update()
    {
        switch (gamePlayMode)
        {
            case GamePlayMode.IN_EVENT:
                CheckCameraZoom();
                break;
            case GamePlayMode.PLAYING:
                nextEventTimer += Time.deltaTime;
                Textos.Instance.ShowNotification = false;
                if (EventsManager.Instance.Events.Count > 0)
                {
                    if (nextEventTimer >= TIME_BETWEEN_EVENTS)
                    {
                        forceEventTimer += Time.deltaTime;
                        Textos.Instance.ShowNotification = true;
                        if (forceEventTimer >= secondsToForceEventIfNointeraction)
                        {
                            LoadNewEvent();
                        }
                    }
                }
                break;
            case GamePlayMode.FREE_MOVEMENT:
                if (ChangingToPlayMode)
                {
                    bool cameraWithZoom = CheckCameraZoom();
                    bool cameraInPos = CheckGameCameraPosition();
                    if (cameraInPos && cameraWithZoom)
                    {   //añadir este codigo tmb a cuando obligamos al player a volver a jugar
                        if (soundManager.currentMusicSource.volume != soundManager.getMusicByAudioSource(soundManager.currentMusicSource).maxVolume)
                        {
                            Debug.Log("ME QUIEREN MATAR AYUDA");
                            Debug.Log("Curr volume" + soundManager.currentMusicSource.volume);
                            Debug.Log("New volume" + soundManager.getMusicByAudioSource(soundManager.currentMusicSource).maxVolume);
                            StartCoroutine(soundManager.MusicFadeIn(2.0f, soundManager.currentMusicSource.volume, soundManager.getMusicByAudioSource(soundManager.currentMusicSource).maxVolume));
                        }
                        gamePlayMode = GamePlayMode.PLAYING;
                        ChangingToPlayMode = false;
                        StateText.text = "Playing";
                    }
                }
                else
                {
                    CheckCameraMovementInput();
                    CheckPlayability();
                }
                break;
        }
    }


    private void CheckCameraMovementInput()
    {
        Vector3 newPosition = Camera.transform.position;
        Vector2 InputVec2D = inputAction_move_camera.ReadValue<Vector2>();
        Vector3 InputVec3D = new(InputVec2D.x, InputVec2D.y, 0);

        newPosition += Time.deltaTime * freeMovementVel * InputVec3D;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        Camera.transform.position = newPosition;
    }

    private void CheckPlayability()
    {

        //inputAction_interact
        float gamepadButtonPressed = inputAction_interact.ReadValue<float>();

        bool canEnterPlayNinja = Camera.transform.localPosition.x <= -15 && gamePlayMode == GamePlayMode.FREE_MOVEMENT;
        // Use manually tracked input device
        if (inputdetector.LastInputDevice == "Gamepad")
        {
            
            PressAgameObject.SetActive(canEnterPlayNinja);
            PressEGameObject.SetActive(false);
        }
        else // Assume Keyboard & Mouse
        {
           
            PressEGameObject.SetActive(canEnterPlayNinja);
            PressAgameObject.SetActive(false);
        }

        // Start game logic for both control schemes
        if (canEnterPlayNinja && inputdetector.LastInputDevice == "KeyboardMouse" && Input.GetKey(KeyCode.E))
        {
            
            ChangingToPlayMode = true;
            PressEGameObject.SetActive(false);
            PressAgameObject.SetActive(false);
        }
        else if (canEnterPlayNinja && inputdetector.LastInputDevice == "Gamepad" && gamepadButtonPressed == 1.0f)
        {
            
            ChangingToPlayMode = true;
            PressAgameObject.SetActive(false);
            PressEGameObject.SetActive(false);
        }


    }

    private bool CheckGameCameraPosition()
    {
        if (Camera.transform.localPosition != CAMERA_POSITION_FOR_GAME)
        {
            Camera.transform.localPosition =
            Vector3.MoveTowards(Camera.transform.localPosition, CAMERA_POSITION_FOR_GAME, Time.deltaTime * 12);
            return false;
        }
        return true;
    }

    private bool CheckCameraZoom()
    {
        if (ChangingToPlayMode)
        {
            if (Camera.orthographicSize > IN_GAME_CAMERA_SIZE)
            {
                Camera.orthographicSize -= Time.deltaTime * 8;
                return false;
            }
            else
            {
                Camera.orthographicSize = IN_GAME_CAMERA_SIZE;
                return true;
            }
        }
        else
        {
            if (Camera.orthographicSize < IN_EVENT_CAMERA_SIZE)
            {
                Camera.orthographicSize += Time.deltaTime * 8;
                return false;
            }
            else
            {
                Camera.orthographicSize = IN_EVENT_CAMERA_SIZE;
                return true;
            }
        }
    }

    public void LoadNewEvent()
    {
        gamePlayMode = GamePlayMode.IN_EVENT;
        StateText.text = "EVENT";
        forceEventTimer = 0;
        nextEventTimer = 0;
        EventsManager.Instance.StartNextEvent();
    }

    private void OnAnyInput(InputControl control)
    {
        var device = control.device;

        if (device is Gamepad)
        {
            LastInputDevice = "Gamepad";
        }
        else if (device is Keyboard || device is Mouse)
        {
            LastInputDevice = "KeyboardMouse";
        }
    }

    private void OnEnable()
    {
        InputActionMap inputActionsMap = inputActions.FindActionMap("Player", throwIfNotFound: true);

        inputAction_move_camera = inputActionsMap.FindAction("Move", throwIfNotFound: true);
        inputAction_move_camera.Enable();
        inputAction_interact = inputActionsMap.FindAction("Jump", throwIfNotFound: true);//same button as dash xd

        inputAction_interact.Enable();
    }

    private void OnDisable()
    {
        inputAction_move_camera.Disable();
        inputAction_interact.Disable();
    }
}
