using UnityEngine;

public class ShakeManager : MonoBehaviour
{
    public Transform cameraTransform;
    public float shakeAmount = 0.05f;
    public float shakeSpeed = 10f;

    private float timer;
    private Vector3 lastShakeOffset;

    void Update()
    {
        cameraTransform.localPosition -= lastShakeOffset;

        timer += Time.deltaTime * shakeSpeed;
        float offsetX = (Mathf.PerlinNoise(timer, 0f) - 0.5f) * shakeAmount;
        float offsetY = (Mathf.PerlinNoise(0f, timer) - 0.5f) * shakeAmount;

        Vector3 newShakeOffset = new(offsetX, offsetY, 0f);

        cameraTransform.localPosition += newShakeOffset;

        lastShakeOffset = newShakeOffset;
    }
}