using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chunk : MonoBehaviour
{
    public List<Chunk> NextPossibleChunks;

    public float GetXSize()
    {
        return GetComponent<RectTransform>().sizeDelta.x;
    }

}
