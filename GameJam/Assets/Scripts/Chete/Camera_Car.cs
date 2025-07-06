using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Collections;

public class Camera_Car : MonoBehaviour
{
    public static float TIME_BETWEEN_EVENTS = 5;
    public Transform LWindow;
    public Transform RWindow;
    public Transform CarCenter;
    public Camera camera;

    public float vel;

    [NonSerialized] public Transform target;
    [NonSerialized] public bool isMoving = false;
    private bool goingToPos2 = true;
    private float moveTimer = 0;

    //public InputActionAsset inputActions;
    //public InputAction inputActionCamera;
    //public float cameraDirectionInput;

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            GoToEvent();
        }
        else
        {
            moveTimer += Time.deltaTime;
            if (moveTimer >= TIME_BETWEEN_EVENTS)
            {
                NewEvent(getNextTarget());
                moveTimer = 0;
            }
        }
    }

    void Start()
    {
        moveTimer = TIME_BETWEEN_EVENTS;
    }

    private Transform getNextTarget()
    {
        if (target == LWindow)
        {
            int index = UnityEngine.Random.Range(0, 2);
            return index == 0 ? CarCenter : RWindow;
        }
        else
        {
            return LWindow;
        }
    }

    void GoToEvent()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, vel * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) == 0)
        {
            isMoving = false;
            if (target == LWindow) GameManager.Instance.gamePlayMode = GamePlayMode.PLAYING;
        }
    }

    void NewEvent(Transform newTarget)
    {
        Debug.Log("Nuevo target");
        target = newTarget;
        isMoving = true;
        if (target != LWindow) GameManager.Instance.gamePlayMode = GamePlayMode.IN_EVENT;
    }

    //private void OnEnable()
    //{
    //    InputActionMap inputActionsMap = inputActions.FindActionMap("Player", throwIfNotFound: true);

    //    if(inputActionsMap != null)
    //    {
    //        inputActionCamera = inputActionsMap.FindAction("Move", throwIfNotFound: false);
    //        if (inputActionCamera != null)
    //            inputActionCamera.Enable();
    //    }

    //}

    //private void OnDisable()
    //{
    //    if (inputActionCamera != null)
    //    {
    //        inputActionCamera.Disable();
    //    }
    //}
}
