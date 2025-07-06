using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chunk : MonoBehaviour
{
    public List<Chunk> NextPossibleChunks;
    public Image BackgroundSizeImage;

    public float GetXSize()
    {
        return BackgroundSizeImage.rectTransform.rect.width;
    }

}
