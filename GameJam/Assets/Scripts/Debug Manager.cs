using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public List<TextMeshProUGUI> LiveChunksTexts, PossibleNextChunksText;
    public TextMeshProUGUI CurrentChunkText;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < LiveChunksTexts.Count; i++)
        {
            bool available = i < ChunksManager.Instance.LiveChunks.Count;
            LiveChunksTexts[i].gameObject.SetActive(available);
            if (available)
            {
                LiveChunksTexts[i].text = ChunksManager.Instance.LiveChunks[i].name.Replace("(Clone)", "");
            }
        }
        for (int i = 0; i < PossibleNextChunksText.Count; i++)
        {
            bool available = i < ChunksManager.Instance.CurrentChunk.NextPossibleChunks.Count;
            PossibleNextChunksText[i].gameObject.SetActive(available);
            if (available)
            {
                PossibleNextChunksText[i].text = ChunksManager.Instance.CurrentChunk.NextPossibleChunks[i].name.Replace("(Clone)", "");
            }
        }
        CurrentChunkText.text = ChunksManager.Instance.CurrentChunk.name.Replace("(Clone)", "");
    }

    public void ChangeSpeed(bool increase)
    {
        ChunksManager.Instance.Speed += increase ? 50 : -50;
    }
}
