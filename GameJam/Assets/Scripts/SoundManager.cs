using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[SerializeField]
public enum MusicTheme
{
    MAIN_MENU,
    GAME_START,
    RADIO_MUSIC
}
[System.Serializable]
public struct GameMusic
{
    public AudioSource music;
    public MusicTheme theme;
}
public class SoundManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static SoundManager Instance;
    public AudioSource currentMusicSource;
    public AudioSource newMusicSource;

    public AudioSource soundFXAudioSource;

    public float musicFadeDuration = 2.0f;
    public float musicFadeOutAndStopDuration = 2.0f;
    public List<GameMusic> GameMusicList;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicate managers
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        foreach (var gameMusic in GameMusicList)
        {
            Debug.Log($"Theme: {gameMusic.theme}, AudioSource: {gameMusic.music}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayOneShotSFX(AudioClip audio)
    {

        soundFXAudioSource.loop = false;
        soundFXAudioSource.PlayOneShot(audio);
    }

    public void PlayMusic(MusicTheme theme_)
    {
        AudioSource selectedMusic = getMusic(theme_);
        //Debug.Log($"Selected music: {selectedMusic}, Clip: {selectedMusic.clip}");
        if (selectedMusic != null)
        {
            newMusicSource = selectedMusic;
            if(currentMusicSource != null)
            {
                StartCoroutine(CrossfadeMusic(currentMusicSource, newMusicSource, musicFadeDuration));
            }
            else
            {
                currentMusicSource = newMusicSource;
                currentMusicSource.volume = 1f;
                currentMusicSource.Play();
            }
            int a = 3;
        }
    }
    public void StopCurrentMusic()
    {
        StartCoroutine(MusicFadeOut(musicFadeOutAndStopDuration));
    }
    IEnumerator MusicFadeOut(float duration)
    {
        float elapsed = 0;       
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float newVolume = Mathf.Lerp(1, 0, t);
            currentMusicSource.volume = newVolume;

            elapsed += Time.deltaTime;

            yield return null;
        }
        currentMusicSource.volume = 0f;
        currentMusicSource.Stop();
      
    }
    IEnumerator CrossfadeMusic(AudioSource from, AudioSource to, float duration)
    {
        float elapsed = 0;
        to.volume = 0f;
        to.Play();
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float fromNewVolume = Mathf.Lerp(1, 0, t);
            from.volume = fromNewVolume;
            float toNewVolume = Mathf.Lerp(0, 1, t);
            to.volume = toNewVolume;
            elapsed += Time.deltaTime;

            yield return null;
        }
        from.volume = 0f;
        to.volume   = 1f;
        from.Stop();
        currentMusicSource = to;
    }
    AudioSource getMusic(MusicTheme theme_)
    {
        AudioSource ret = null;
        for(int i = 0; i < GameMusicList.Count; i++)
        {
            if (GameMusicList[i].theme == theme_)
            {               
                return GameMusicList[i].music;
            }
        }
       
        return null;
    }
}
