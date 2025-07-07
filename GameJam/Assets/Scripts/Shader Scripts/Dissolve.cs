using System.Collections;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    public float dissolveTime = 0.75f;
    public Material material;
    private int dissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private int verticalDissolveAmount = Shader.PropertyToID("_VerticalDissolve");

    public bool UseVerticalDissolve;

    public void StartVanishing()
    {
        StartCoroutine(Vanish());
    }
    public void StartAppear()
    {
        StartCoroutine(Appear());
    }

    private IEnumerator Vanish()
    {
        float elapsedTime = 0f;
        while (elapsedTime <= dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedDissolve = Mathf.Lerp(0, 1.1f, elapsedTime / dissolveTime);

            material.SetFloat(dissolveAmount, lerpedDissolve);
            if (UseVerticalDissolve) material.SetFloat(verticalDissolveAmount, lerpedDissolve);
            yield return null;
        }
    }

    private IEnumerator Appear()
    {
        float elapsedTime = 0f;
        while (elapsedTime <= dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedDissolve = Mathf.Lerp(1.1f, 0, elapsedTime / dissolveTime);

            material.SetFloat(dissolveAmount, lerpedDissolve);
            if (UseVerticalDissolve) material.SetFloat(verticalDissolveAmount, lerpedDissolve);
            yield return null;
        }
    }
}
