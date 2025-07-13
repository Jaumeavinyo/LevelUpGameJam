using System.Collections.Generic;
using UnityEngine;

public class BackgroundChunk : MonoBehaviour
{
    void Start()
    {
    }
    public float GetXSize()
    {
        RectTransform rect = GetComponent<RectTransform>();
        return rect.sizeDelta.x * rect.localScale.x;
    }
}
