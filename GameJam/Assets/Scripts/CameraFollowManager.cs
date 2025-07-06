using UnityEngine;

public class CameraFollowManager : MonoBehaviour
{
    public Camera Camera;
    public float maxX, maxY, minX, minY;
    public float Speed;
    public GameObject Character;

    void Update()
    {
        if (Character.transform.localPosition.x > 7)
        {
            if (Camera.transform.localPosition.x < maxX)
            {
                Camera.transform.localPosition =
                Vector3.Lerp(Camera.transform.localPosition, new Vector3(maxX, Camera.transform.localPosition.y, -10), Speed * Time.deltaTime);
            }
        }
        else if (Character.transform.localPosition.x < -7)
        {
            if (Camera.transform.localPosition.x > minX)
            {
                Camera.transform.localPosition =
                Vector3.Lerp(Camera.transform.localPosition, new Vector3(minX, Camera.transform.localPosition.y, -10), Speed * Time.deltaTime);
            }
        }
        else
        {
            Camera.transform.localPosition =
            Vector3.Lerp(Camera.transform.localPosition, new Vector3(0, Camera.transform.localPosition.y, -10), Speed * Time.deltaTime);
        }

        if (Character.transform.localPosition.y > 5)
        {
            if (Camera.transform.localPosition.y < maxY)
            {
                Camera.transform.localPosition =
                Vector3.Lerp(Camera.transform.localPosition, new Vector3(Camera.transform.localPosition.x, maxY, -10), Speed * Time.deltaTime);
            }
        }
        else if (Character.transform.localPosition.y < -5)
        {
            if (Camera.transform.localPosition.y > minY)
            {
                Camera.transform.localPosition =
                Vector3.Lerp(Camera.transform.localPosition, new Vector3(Camera.transform.localPosition.x, minY, -10), Speed * Time.deltaTime);
            }
        }
        else
        {
            Camera.transform.localPosition =
            Vector3.Lerp(Camera.transform.localPosition, new Vector3(Camera.transform.localPosition.x, 0, -10), Speed * Time.deltaTime);
        }
    }
}
