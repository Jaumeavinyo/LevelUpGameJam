using System;
using System.Collections.Generic;
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
        return IsRightBackgroundManager ? chunk.transform.localPosition.x >= 32 : chunk.transform.localPosition.x <= -28;
    }

    private bool CheckGeneratePosition(BackgroundChunk chunk)
    {
        return IsRightBackgroundManager ? chunk.transform.localPosition.x >= 16 : chunk.transform.localPosition.x <= -12;
    }

    void Update()
    {
        Display.gameObject.SetActive(IsRightBackgroundManager ? GameManager.Instance.Camera.transform.localPosition.x > 0 : GameManager.Instance.Camera.transform.localPosition.x < 0);
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
        NewBackground.transform.localPosition = CurrentBackground.transform.localPosition + new Vector3(IsRightBackgroundManager ? -40 : 40, 0, 0);
        CurrentBackground = NewBackground;
    }
}
