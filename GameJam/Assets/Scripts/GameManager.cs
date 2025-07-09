using UnityEngine;
using System;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.UI;
using System.Collections.Generic;

public enum GamePlayMode
{
    PLAYING,
    IN_EVENT,
    FREE_MOVEMENT
}

[Serializable]
public struct StructEvent
{

    [Tooltip("The camera position to move to at this time")]
    public Transform CameraPosition;

    public string Dialogue;
    public float EventDuration;
    [NonSerialized] public bool IsShortEvent;
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

    public static GameManager Instance;
    public TextMeshProUGUI StateText;
    public static Vector3 CAMERA_POSITION_FOR_GAME = new(-16.5f, 1, -1);
    public Transform LWindow, RWindow, CarCenter;
    public Camera Camera;
    public static float IN_GAME_CAMERA_SIZE = 4f, IN_EVENT_CAMERA_SIZE = 6;
    public static float TIME_BETWEEN_EVENTS = 20;
    public float vel, freeMovementVel;

    [NonSerialized] public Transform target;
    [NonSerialized] public bool isMoving = false;
    private float nextEventTimer = 0, eventTimer = 0, forceEventTimer = 0;
    [NonSerialized] public GamePlayMode gamePlayMode;
    public GameObject PressEGameObject, PressAgameObject;

    public float minX = -18, maxX = 18, minY = -1, maxY = 1;
    private bool ChangingToPlayMode = false, InShortEvent = false;
    public Textos TextScript;
    public GameObject TextManager;
    public EventsManager eventsManager;
    void Start()
    {
        gamePlayMode = GamePlayMode.FREE_MOVEMENT;
        playerName = PlayerData.playerName;
        Debug.Log(playerName);
        TextScript = TextManager.GetComponent<Textos>();

    }

    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        switch (gamePlayMode)
        {
            case GamePlayMode.IN_EVENT:
                if (isMoving)
                {
                    GoToEvent();
                }
                else if (eventTimer > 0)
                {
                    eventTimer -= Time.deltaTime;
                }
                else
                {
                    if (InShortEvent)
                    {
                        StateText.text = "Back to Game";
                        ChangingToPlayMode = true;
                    }
                    else
                    {
                        StateText.text = "Free Movement";
                    }
                    gamePlayMode = GamePlayMode.FREE_MOVEMENT;
                    TextScript.FinishEvent();
                }
                CheckCameraZoom();
                break;
            case GamePlayMode.PLAYING:
                nextEventTimer += Time.deltaTime;
                TextScript.ShowNotification = false;
                if (eventsManager.Events.Count > 0)
                {
                    if (nextEventTimer >= TIME_BETWEEN_EVENTS)
                    {
                        forceEventTimer += Time.deltaTime;
                        TextScript.ShowNotification = true;
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
                    bool cameraInPos = CheckGameCameraPosition();
                    bool cameraWithZoom = CheckCameraZoom();
                    if (cameraInPos && cameraWithZoom)
                    {
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
        if (InputDeviceDetector.LastInputDevice == "Gamepad")
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
        if (canEnterPlayNinja && InputDeviceDetector.LastInputDevice == "KeyboardMouse" && Input.GetKey(KeyCode.E))
        {
            ChangingToPlayMode = true;
            PressEGameObject.SetActive(false);
            PressAgameObject.SetActive(false);
        }
        else if (canEnterPlayNinja && InputDeviceDetector.LastInputDevice == "Gamepad" && gamepadButtonPressed == 1.0f)
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

    void GoToEvent()
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, -1);
        Camera.transform.position = Vector3.MoveTowards(Camera.transform.position, targetPosition, vel * Time.deltaTime);

        if (Vector3.Distance(Camera.transform.position, targetPosition) == 0)
        {
            isMoving = false;
        }
    }

    public void LoadNewEvent()
    {
        StructEvent newEvent = eventsManager.Events[0];
        target = newEvent.CameraPosition;
        isMoving = true;
        gamePlayMode = GamePlayMode.IN_EVENT;
        StateText.text = "EVENT";
        TextScript.OpenEvent(newEvent.Dialogue);
        eventsManager.Events.Remove(newEvent);
        forceEventTimer = 0;
        nextEventTimer = 0;
        InShortEvent = newEvent.IsShortEvent;
        eventTimer = newEvent.EventDuration;
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
