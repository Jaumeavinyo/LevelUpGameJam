using UnityEngine;
using UnityEngine.UI;


public class ExclamacionSizeChange : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Image exclamacion;
    [Header("Scale Settings")]
    public float pulseSpeed = 2f;  // Animation speed
    public float minScale = 0.8f;  // Smallest allowed scale
    public float maxScale = 1.2f;  // Largest allowed scale

    void Update()
    {
        if (exclamacion != null)
        {
            // Get raw sine wave value (-1 to 1)
            float sineValue = Mathf.Sin(Time.time * pulseSpeed);

            // Remap sine wave from (-1 to 1) to (0 to 1)
            float normalizedValue = (sineValue + 1f) / 2f;

            // Apply scale within capped range
            float currentScale = Mathf.Lerp(minScale, maxScale, normalizedValue);

            // Apply to both X and Y axes
            exclamacion.transform.localScale = new Vector3(currentScale, currentScale, 1f);
        }
    }
}
