using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public List<Transform> targetTransforms;
    public float Speed = 100;
    private Transform TargetTransform;
    public GameObject MainHolder;

    void Start()
    {
        TargetTransform = targetTransforms[0];
    }
    public void SetTargetTransform(int index)
    {
        if (index < targetTransforms.Count)
        {
            TargetTransform = targetTransforms[index];
        }
    }

    void Update()
    {
        if (MainHolder.transform.position != TargetTransform.position)
        {
            MainHolder.transform.position = Vector3.Lerp(MainHolder.transform.position, TargetTransform.position, Speed * Time.deltaTime);

        }
    }
}