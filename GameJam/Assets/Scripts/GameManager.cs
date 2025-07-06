using System;
using UnityEngine;

public enum GamePlayMode
{
    PLAYING,
    IN_EVENT
}

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameManager Instance;
    public static float IN_GAME_CAMERA_SIZE = 3.5f;
    public static float IN_EVENT_CAMERA_SIZE = 6;
    public Camera_Car Cam;

    [NonSerialized] public GamePlayMode gamePlayMode;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gamePlayMode = GamePlayMode.IN_EVENT;
    }

    // Update is called once per frame
    void Update()
    {
        if (gamePlayMode == GamePlayMode.PLAYING)
        {
            if (Cam.transform.localPosition.y != 1)
            {
                Cam.transform.localPosition =
                Vector3.MoveTowards(Cam.transform.localPosition, new Vector3(Cam.transform.position.x, 1, Cam.transform.position.z), Time.deltaTime);
            }
            if (Cam.camera.orthographicSize > IN_GAME_CAMERA_SIZE)
            {
                Cam.camera.orthographicSize -= Time.deltaTime;
            }
            else
            {
                Cam.camera.orthographicSize = IN_GAME_CAMERA_SIZE;
            }
        }
        else if (gamePlayMode == GamePlayMode.IN_EVENT)
        {
            if (Cam.transform.localPosition.y != 0)
            {
                Cam.transform.localPosition =
                Vector3.MoveTowards(Cam.transform.localPosition, new Vector3(Cam.transform.position.x, 0, Cam.transform.position.z), Time.deltaTime);
            }
            if (Cam.camera.orthographicSize < IN_EVENT_CAMERA_SIZE)
            {
                Cam.camera.orthographicSize += Time.deltaTime;
            }
            else
            {
                Cam.camera.orthographicSize = IN_EVENT_CAMERA_SIZE;
            }
        }
    }
}
