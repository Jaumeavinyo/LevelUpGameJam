using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Camera_Car Cam;

    public GameObject carBackground;

    bool gameplayRotation;
    void Start()
    {
        gameplayRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Cam.isMoving)
        {
            if (Cam.transform.position == Cam.LWindow.transform.position && !gameplayRotation) 
            {
                gameplayRotation = true;
                carBackground.transform.Rotate(0, 0, 5);
            }
                 
        }
    }
}
