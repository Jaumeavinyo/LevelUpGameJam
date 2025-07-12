using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    public static FadeToBlack Instance;
    public Image Panel;
    public float FadeDuration = 1f;
    private bool isFading;
    private float targetAlpha;
    private float fadeSpeed;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isFading)
        {
            Color currentColor = Panel.color;
            float newAlpha = Mathf.MoveTowards(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            Panel.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);

            if (Mathf.Approximately(newAlpha, targetAlpha))
                isFading = false;
        }
    }

    public void FadeIn()
    {
        StartFade(0f);
    }

    public void FadeOut()
    {
        StartFade(1f);
    }

    private void StartFade(float alpha)
    {
        targetAlpha = alpha;
        fadeSpeed = 1f / FadeDuration;
        isFading = true;
    }
}
