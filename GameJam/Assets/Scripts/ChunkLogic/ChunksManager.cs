using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksManager : MonoBehaviour
{
    public static ChunksManager Instance;
    public Chunk StartingChunk;
    [NonSerialized] public List<Chunk> LiveChunks = new();
    [NonSerialized] public Chunk CurrentChunk;
    public float baseSpeed = 2, baseAcceleration = 0.05f, MAX_SPEED = 3;
    [NonSerialized] public float Acceleration, Speed;

    public GameObject Display;
    private bool GameStarted = false;
    public Transform PlayerStartingPoint;

    public FSM_CharMovement Character;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Acceleration = baseAcceleration;
        Speed = baseSpeed;
        Character.transform.localPosition = PlayerStartingPoint.transform.localPosition;
        Chunk firstChunk = Instantiate(StartingChunk, Display.transform);
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
        Chunk newChunk = Instantiate(selectedTemplate, Display.transform);
        LiveChunks.Add(newChunk);
        newChunk.transform.localPosition = CurrentChunk.transform.localPosition + new Vector3(CurrentChunk.GetXSize() / 2, 0, 0) + new Vector3(newChunk.GetXSize() / 2, 0, 0);
        CurrentChunk = newChunk;
    }

    private void RestartGame()
    {
        GameStarted = true;
        Character.transform.localPosition = PlayerStartingPoint.transform.localPosition;
        Rigidbody2D rb = Character.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        Character.GetComponent<Dissolve>().StartAppear();

    }

    public void StartRestartCountdown()
    {
        if (GameStarted)
        {
            Character.GetComponent<Dissolve>().StartVanishing(false);
            StartCoroutine(RestartAfterDelay());
        }
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        RestartGame();
    }

    void Update()
    {
        bool GameMangerPlaying = GameManager.Instance == null || GameManager.Instance.gamePlayMode == GamePlayMode.PLAYING;
        if (GameMangerPlaying)
        {
            if (!GameStarted)
            {
                RestartGame();
            }
            {

                Character.gameObject.transform.localPosition += new Vector3(-Speed * Time.deltaTime, 0, 0);
                Speed += Time.deltaTime * Acceleration;
                Speed = Math.Min(Speed, MAX_SPEED);
            }
        }
        else if (GameStarted)
        {
            Character.GetComponent<Dissolve>().StartVanishing(true);
            GameStarted = false;
        }
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


    }
}
