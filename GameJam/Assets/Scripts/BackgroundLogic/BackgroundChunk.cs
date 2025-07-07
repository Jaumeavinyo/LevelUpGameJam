using System.Collections.Generic;
using UnityEngine;

public class BackgroundChunk : MonoBehaviour
{
    public List<BackgroundChunk> NextPossibleBackgrounds;
    void Start()
    {
        GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }
    public float GetXSize()
    {
        return GetComponent<RectTransform>().sizeDelta.x;
    }
}
