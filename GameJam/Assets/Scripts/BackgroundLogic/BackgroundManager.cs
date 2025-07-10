using System;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public BackgroundChunk StartingBackground;
    [NonSerialized] public List<BackgroundChunk> LiveBackgrounds = new();
    private BackgroundChunk CurrentBackground;
    public GameObject Display;

    void Start()
    {
        BackgroundChunk FirstBackground = Instantiate(StartingBackground, Display.transform);
        LiveBackgrounds.Add(FirstBackground);
        CurrentBackground = FirstBackground;
        GenerateRandomNextBackground();
        GenerateRandomNextBackground();
    }

    void Update()
    {
        List<BackgroundChunk> CurrentLiveBackgrounds = new(LiveBackgrounds);
        foreach (BackgroundChunk chunk in CurrentLiveBackgrounds)
        {
            chunk.transform.localPosition += new Vector3(-ChunksManager.Instance.Speed * Time.deltaTime, 0, 0);
            if (chunk.transform.localPosition.x <= -30)
            {
                Destroy(chunk.gameObject);
                LiveBackgrounds.Remove(chunk);
            }
        }
        if (CurrentBackground.transform.localPosition.x <= 11) GenerateRandomNextBackground();
        ChunksManager.Instance.Speed += Time.deltaTime * ChunksManager.Instance.Acceleration;
    }

    public void GenerateRandomNextBackground()
    {
        if (CurrentBackground == null || CurrentBackground.NextPossibleBackgrounds == null || CurrentBackground.NextPossibleBackgrounds.Count == 0)
            throw new ArgumentException("PreviousChunk or its NextPossibleChunks is null or empty.");

        int index = UnityEngine.Random.Range(0, CurrentBackground.NextPossibleBackgrounds.Count);
        BackgroundChunk selectedTemplate = CurrentBackground.NextPossibleBackgrounds[index];
        BackgroundChunk NewBackground = Instantiate(selectedTemplate, Display.transform);
        LiveBackgrounds.Add(NewBackground);
        NewBackground.transform.localPosition = CurrentBackground.transform.localPosition + new Vector3(CurrentBackground.GetXSize() / 2, 0, 0) + new Vector3(NewBackground.GetXSize() / 2, 0, 0);
        CurrentBackground = NewBackground;
    }
}
