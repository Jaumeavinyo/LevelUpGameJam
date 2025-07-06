using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunksManager : MonoBehaviour
{
    public static ChunksManager Instance;
    public Chunk StartingChunk;
    [NonSerialized] public List<Chunk> LiveChunks = new();
    [NonSerialized] public Chunk CurrentChunk;
    public Transform ChunksHolder;
    [NonSerialized] public float Speed = 2, Acceleration = 0.1f;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Chunk firstChunk = Instantiate(StartingChunk, ChunksHolder);
        LiveChunks.Add(firstChunk);
        CurrentChunk = firstChunk;
        GenerateRandomNextChunk();
        GenerateRandomNextChunk();
    }

    public void GenerateRandomNextChunk()
    {
        if (CurrentChunk == null || CurrentChunk.NextPossibleChunks == null || CurrentChunk.NextPossibleChunks.Count == 0)
            throw new ArgumentException("PreviousChunk or its NextPossibleChunks is null or empty.");

        int index = UnityEngine.Random.Range(0, CurrentChunk.NextPossibleChunks.Count);
        Chunk selectedTemplate = CurrentChunk.NextPossibleChunks[index];
        Chunk newChunk = Instantiate(selectedTemplate, ChunksHolder);
        LiveChunks.Add(newChunk);
        newChunk.transform.localPosition = CurrentChunk.transform.localPosition + new Vector3(CurrentChunk.GetXSize() / 2, 0, 0) + new Vector3(newChunk.GetXSize() / 2, 0, 0);
        CurrentChunk = newChunk;
    }

    void Update()
    {
        List<Chunk> CurrentLiveChunks = new(LiveChunks);
        foreach (Chunk chunk in CurrentLiveChunks)
        {
            chunk.transform.localPosition += new Vector3(-Speed * Time.deltaTime, 0, 0);
            if (chunk.transform.localPosition.x <= -30)
            {
                Destroy(chunk.gameObject);
                LiveChunks.Remove(chunk);
            }
        }
        if (CurrentChunk.transform.localPosition.x <= 11) GenerateRandomNextChunk();
        Speed += Time.deltaTime * Acceleration;
    }
}
