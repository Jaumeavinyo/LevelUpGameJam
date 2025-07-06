using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Collections;

public class Camera_Car : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform PosEvento1;
    public Transform PosEvento2;
    public float vel;

    private Transform target;
    private bool isMoving = false;
    private bool goingToPos2 = true;

    //public InputActionAsset inputActions;
    //public InputAction inputActionCamera;
    //public float cameraDirectionInput;

    void Start()
    {        
        StartCoroutine(Sequence());
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            GoToEvent();
        }
    }
    
    void GoToEvent()
    {
        Debug.Log("Moviéndose hacia el objetivo...");
        transform.position = Vector3.MoveTowards(transform.position, target.position, vel * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            isMoving = false;
        }
        
    }
    void NewEvent(Transform newTarget)
    {
        Debug.Log("Nuevo target");
        target = newTarget;
        isMoving = true;
    }
    IEnumerator Sequence()
    {
        for (int i = 0; i < 3; i++)
        {
            NewEvent(PosEvento1);
            yield return new WaitUntil(() => !isMoving);
            yield return new WaitForSeconds(5f);

            NewEvent(PosEvento2);
            yield return new WaitUntil(() => !isMoving);
            yield return new WaitForSeconds(5f);
        }
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
