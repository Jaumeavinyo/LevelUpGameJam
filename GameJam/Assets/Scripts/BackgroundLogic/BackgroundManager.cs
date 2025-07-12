using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public BackgroundChunk StartingBackground, SecondBackground, ThirdBackground;
    [NonSerialized] public List<BackgroundChunk> LiveBackgrounds = new();
    private BackgroundChunk CurrentBackground;
    public GameObject Display;
    public bool IsRightBackgroundManager;

    void Start()
    {
        BackgroundChunk FirstBackground = Instantiate(StartingBackground, Display.transform);
        FirstBackground.transform.localPosition = Vector3.zero;
        LiveBackgrounds.Add(FirstBackground);
        CurrentBackground = FirstBackground;
    }

    private bool CheckDestroyPosition(BackgroundChunk chunk)
    {
        return IsRightBackgroundManager ? chunk.transform.localPosition.x >= 40 : chunk.transform.localPosition.x <= -30;
    }

    private bool CheckGeneratePosition(BackgroundChunk chunk)
    {
        return IsRightBackgroundManager ? chunk.transform.localPosition.x >= 20 : chunk.transform.localPosition.x <= 11;
    }

    void Update()
    {
        List<BackgroundChunk> CurrentLiveBackgrounds = new(LiveBackgrounds);
        foreach (BackgroundChunk chunk in CurrentLiveBackgrounds)
        {
            chunk.transform.localPosition += new Vector3(ChunksManager.Instance.Speed * Time.deltaTime * (IsRightBackgroundManager ? 1 : -1), 0, 0);

            if (CheckDestroyPosition(chunk))
            {
                Destroy(chunk.gameObject);
                LiveBackgrounds.Remove(chunk);
            }
        }
        if (CheckGeneratePosition(CurrentBackground)) GenerateNextBackground();
    }

    public void GenerateNextBackground()
    {
        BackgroundChunk selectedTemplate = EventsManager.Instance.currentGamePhase switch
        {
            CurrentGamePhase.First => StartingBackground,
            CurrentGamePhase.Second => SecondBackground,
            _ => ThirdBackground
        };
        BackgroundChunk NewBackground = Instantiate(selectedTemplate, Display.transform);
        LiveBackgrounds.Add(NewBackground);
        NewBackground.transform.localPosition = CurrentBackground.transform.localPosition + (new Vector3(CurrentBackground.GetXSize() / 2f, 0, 0) + new Vector3(NewBackground.GetXSize() / 2f, 0, 0)) * (IsRightBackgroundManager ? -1 : 1);
        CurrentBackground = NewBackground;
    }
}
