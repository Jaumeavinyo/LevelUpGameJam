using System.Collections.Generic;
using UnityEngine;

public class BackgroundChunk : MonoBehaviour
{
    public List<BackgroundChunk> NextPossibleBackgrounds;

    public float GetXSize()
    {
        return GetComponent<RectTransform>().sizeDelta.x;
    }
}
