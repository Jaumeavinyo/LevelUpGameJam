using UnityEngine;
using System;
using UnityEngine.AI;
using TMPro;

public enum GamePlayMode
{
    PLAYING,
    IN_EVENT,
    FREE_MOVEMENT
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TextMeshProUGUI StateText;
    public static float TIME_BETWEEN_EVENTS = 10;
    public static Vector3 CAMERA_POSITION_FOR_GAME = new(-16.5f, 1, -1);
    public Transform LWindow;
    public Transform RWindow;
    public Transform CarCenter;
    public Camera Camera;
    public static float IN_GAME_CAMERA_SIZE = 3.5f;
    public static float IN_EVENT_CAMERA_SIZE = 6;

    public float vel, freeMovementVel;

    [NonSerialized] public Transform target;
    [NonSerialized] public bool isMoving = false;
    private float moveTimer = 0, eventTimer = 0;
    [NonSerialized] public GamePlayMode gamePlayMode;
    public GameObject PressEGameObject;

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

        if (Input.GetKey(KeyCode.W))
        {
            newPosition += Time.deltaTime * freeMovementVel * new Vector3(0, 1f, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            newPosition += Time.deltaTime * freeMovementVel * new Vector3(-1f, 0, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            newPosition += Time.deltaTime * freeMovementVel * new Vector3(0, -1f, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            newPosition += Time.deltaTime * freeMovementVel * new Vector3(1f, 0, 0);
        }

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        Camera.transform.position = newPosition;
    }


    private void CheckPlayability()
    {
        bool canPressE = Camera.transform.localPosition.x <= -15 && gamePlayMode == GamePlayMode.FREE_MOVEMENT;
        PressEGameObject.SetActive(canPressE);
        if (canPressE && Input.GetKey(KeyCode.E))
        {
            ChangingToPlayMode = true;
            PressEGameObject.SetActive(false);
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
}
