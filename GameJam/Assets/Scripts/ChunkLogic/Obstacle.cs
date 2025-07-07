using UnityEngine;

public class Obstacle : MonoBehaviour
{
    void Start()
    {
        GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }
}