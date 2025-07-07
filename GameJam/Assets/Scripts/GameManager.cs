using UnityEngine;
using System;
using UnityEngine.AI;
using TMPro;
using UnityEngine.InputSystem;

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
    public InputAction inputAction_move_camera;
    public InputAction inputAction_interact;
    public static string LastInputDevice = "None";

    public string playerName;

    public static GameManager Instance;
    public TextMeshProUGUI StateText;
    public static float TIME_BETWEEN_EVENTS = 10;
    public static Vector3 CAMERA_POSITION_FOR_GAME = new(-16.5f, 1, -1);
    public Transform LWindow;
    public Transform RWindow;
    public Transform CarCenter;
    public Camera Camera;
    public static float IN_GAME_CAMERA_SIZE = 4f;
    public static float IN_EVENT_CAMERA_SIZE = 6;

    public float vel, freeMovementVel;

    [NonSerialized] public Transform target;
    [NonSerialized] public bool isMoving = false;
    private float moveTimer = 0, eventTimer = 0;
    [NonSerialized] public GamePlayMode gamePlayMode;
    public GameObject PressEGameObject;
    public GameObject PressAgameObject;

    public float minX = -18, maxX = 18;
    public float minY = -1, maxY = 1;
    private bool ChangingToPlayMode = false;

    //public InputActionAsset inputActions;
    //public InputAction inputActionCamera;
    //public float cameraDirectionInput;

    // Update is called once per frame
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
                    gamePlayMode = GamePlayMode.FREE_MOVEMENT;
                    StateText.text = "Free Movement";
                }
                CheckCameraZoom();
                break;
            case GamePlayMode.PLAYING:
                moveTimer += Time.deltaTime;
                if (moveTimer >= TIME_BETWEEN_EVENTS)
                {
                    NewEvent(getNextTarget());
                    moveTimer = 0;
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
        Vector3 InputVec3D = new Vector3(InputVec2D.x, InputVec2D.y, 0);

        newPosition += Time.deltaTime * freeMovementVel * InputVec3D;
        //if (Input.GetKey(KeyCode.W))
        //{
        //    newPosition += Time.deltaTime * freeMovementVel * new Vector3(0, 1f, 0);
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    newPosition += Time.deltaTime * freeMovementVel * new Vector3(-1f, 0, 0);
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    newPosition += Time.deltaTime * freeMovementVel * new Vector3(0, -1f, 0);
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    newPosition += Time.deltaTime * freeMovementVel * new Vector3(1f, 0, 0);
        //}

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        Camera.transform.position = newPosition;
    }


    private void CheckPlayability()
    {
        //inputAction_interact
        float gamepadButtonPressed = inputAction_interact.ReadValue<float>();

        bool canEnterPlayNinja = Camera.transform.localPosition.x <= -15 && gamePlayMode == GamePlayMode.FREE_MOVEMENT;
        // Detect keyboard 
        if (Input.anyKeyDown || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            LastInputDevice = "KeyboardMouse";
            PressEGameObject.SetActive(canEnterPlayNinja);
            PressAgameObject.SetActive(false);
        }

        // Detect gamepad 
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            LastInputDevice = "Gamepad";
            PressAgameObject.SetActive(canEnterPlayNinja);
            PressEGameObject.SetActive(false);
        }

        
        
        if (canEnterPlayNinja && Input.GetKey(KeyCode.E))
        {
            ChangingToPlayMode = true;
           
            PressEGameObject.SetActive(false);
        }else if(canEnterPlayNinja && gamepadButtonPressed==1.0)
        {
            ChangingToPlayMode = true;
            PressAgameObject.SetActive(false);
        }
    }

    private bool CheckGameCameraPosition()
    {
        if (Camera.transform.localPosition != CAMERA_POSITION_FOR_GAME)
        {
            Camera.transform.localPosition =
            Vector3.MoveTowards(Camera.transform.localPosition, CAMERA_POSITION_FOR_GAME, Time.deltaTime * 4);
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
                Camera.orthographicSize -= Time.deltaTime * 4;
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
                Camera.orthographicSize += Time.deltaTime * 4;
                return false;
            }
            else
            {
                Camera.orthographicSize = IN_EVENT_CAMERA_SIZE;
                return true;
            }
        }
    }

    void Start()
    {
        gamePlayMode = GamePlayMode.FREE_MOVEMENT;
        playerName = PlayerData.playerName;
        Debug.Log(playerName);
    }

    void Awake()
    {
        Instance = this;
    }

    private Transform getNextTarget()
    {
        int index = UnityEngine.Random.Range(0, 2);
        return index == 0 ? CarCenter : RWindow;
    }

    void GoToEvent()
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, -1);
        Camera.transform.position = Vector3.MoveTowards(Camera.transform.position, targetPosition, vel * Time.deltaTime);

        if (Vector3.Distance(Camera.transform.position, targetPosition) == 0)
        {
            isMoving = false;
            eventTimer = 3;
        }
    }

    void NewEvent(Transform newTarget)
    {
        target = newTarget;
        isMoving = true;
        gamePlayMode = GamePlayMode.IN_EVENT;
        StateText.text = "EVENT";
    }


    private void OnEnable()
    {
        InputActionMap inputActionsMap = inputActions.FindActionMap("Player", throwIfNotFound: true);

        inputAction_move_camera = inputActionsMap.FindAction("Move", throwIfNotFound: true);
        inputAction_move_camera.Enable();
        inputAction_interact = inputActionsMap.FindAction("Dash", throwIfNotFound: true);//same button as dash xd
        inputAction_interact.Enable();
    }

    private void OnDisable()
    {
        inputAction_move_camera.Disable();
        inputAction_interact.Disable();

    }
}
