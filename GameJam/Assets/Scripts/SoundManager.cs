using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[SerializeField]
public enum MusicTheme
{
    NONE,
    MAIN_MENU,
    GAME_START,
    RADIO_MUSIC
}
[System.Serializable]
public class GameMusic
{
    public AudioSource music;
    public MusicTheme theme;
    public float maxVolume;
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

    public void changeMaxVolume(float newMaxVolume)
    {
        for (int i = 0; i < GameMusicList.Count; i++)
        {         
            GameMusicList[i].maxVolume = newMaxVolume;           
        }
    }

    public void changeAllMusicVolume(float Volume)
    {
        for (int i = 0; i < GameMusicList.Count; i++)
        {
            GameMusicList[i].music.volume = Volume;
        }
    }

    public void changeOneSongMaxVolume(MusicTheme musicTheme,float newMaxVolume)
    {
        for(int i = 0;i< GameMusicList.Count; i++)
        {
            if (GameMusicList[i].theme == musicTheme)
            {              
                GameMusicList[i].maxVolume = newMaxVolume;
            }
        }
    }

    public void PlayOneShotSFX(AudioClip audio)
    {
        soundFXAudioSource.loop = false;
        soundFXAudioSource.PlayOneShot(audio);
    }

    public void PlayMusic(MusicTheme theme_)
    {
        AudioSource selectedMusic = getMusicBytheme(theme_).music;
        //Debug.Log($"Selected music: {selectedMusic}, Clip: {selectedMusic.clip}");
        if (selectedMusic != null)
        {
            newMusicSource = selectedMusic;
            if (currentMusicSource != null)
            {
                StartCoroutine(CrossfadeMusic(currentMusicSource, newMusicSource, musicFadeDuration));
            }
            else
            {

                currentMusicSource = newMusicSource;
                currentMusicSource.volume = getMusicByAudioSource(currentMusicSource).maxVolume;
                currentMusicSource.Play();
            }
            
        }
    }
    public void StopCurrentMusic(float fadeDuration)
    {
        StartCoroutine(MusicFadeOut(fadeDuration, currentMusicSource.volume, 0.0f));
    }

    public IEnumerator MusicFadeOut(float duration,float from, float to)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float newVolume = Mathf.Lerp(from, to, t);
            currentMusicSource.volume = newVolume;

            elapsed += Time.deltaTime;

            yield return null;
        }
        currentMusicSource.volume = to;
        if(currentMusicSource.volume == 0)
        {
            currentMusicSource.Stop();
        }
    }

    public IEnumerator MusicFadeIn(float duration, float from, float to)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float newVolume = Mathf.Lerp(from, to, t);
            currentMusicSource.volume = newVolume;

            elapsed += Time.deltaTime;

            yield return null;
        }
        currentMusicSource.volume = to;
    }
    public IEnumerator CrossfadeMusic(AudioSource from, AudioSource to, float duration)
    {
        //GET BOTH MAX VOLUME
        float fromMaxVolume = 1.0f;
        float toMaxVolume= 1.0f;
        for (int i = 0; i < GameMusicList.Count; i++)
        {
            if (GameMusicList[i].music == from)
            {
                fromMaxVolume = GameMusicList[i].maxVolume;
            }
        }
        for (int i = 0; i < GameMusicList.Count; i++)
        {
            if (GameMusicList[i].music == to)
            {
                toMaxVolume = GameMusicList[i].maxVolume;
            }
        }


        float elapsed = 0;
        to.volume = 0f;
        to.Play();
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float fromNewVolume = Mathf.Lerp(fromMaxVolume, 0, t);
            from.volume = fromNewVolume;
            float toNewVolume = Mathf.Lerp(0, toMaxVolume, t);
            to.volume = toNewVolume;
            elapsed += Time.deltaTime;

            yield return null;
        }
        from.volume = 0f;
        to.volume = toMaxVolume;
        from.Stop();
        currentMusicSource = to;
    }
    public GameMusic getMusicBytheme(MusicTheme theme_)
    {
      
        for (int i = 0; i < GameMusicList.Count; i++)
        {
            if (GameMusicList[i].theme == theme_)
            {
                return GameMusicList[i];
            }
        }

        return null;
    }
    public GameMusic getMusicByAudioSource(AudioSource ASource)
    {
   
        for (int i = 0; i < GameMusicList.Count; i++)
        {
            if (GameMusicList[i].music == ASource)
            {
                return GameMusicList[i];
            }
        }

        return null;
    }
}
