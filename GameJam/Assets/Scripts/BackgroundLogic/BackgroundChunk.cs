using System.Collections.Generic;
using UnityEngine;

public class BackgroundChunk : MonoBehaviour
{
    void Start()
    {
        GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }
    public float GetXSize()
    {
        RectTransform rect = GetComponent<RectTransform>();
        return rect.sizeDelta.x * rect.localScale.x;
    }
}
