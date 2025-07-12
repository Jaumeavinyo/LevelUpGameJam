using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using TMPro;



public class MainMenuUI : MonoBehaviour
{
  

    public Button StartGame;
    public Button Options;//volume credits
    public Button Quit;


    public GameObject nameInputPopup;
    public GameObject optionsPopup;
    public GameObject creditsPanel;
    public GameObject mainPanel;

    public Button CreditsButton;
    public Button BackFromOptionsButton;
    public Button BackfromCreditsButton;
    public Slider volumeSlider;

    public TMP_InputField InputNameUI;
    public Button AcceptInputName;
    public string PlayerName;

    public SoundManager soundManager;

    
    void Start()
    {
        StartGame.onClick.AddListener(OnStartGameClicked);
        Options.onClick.AddListener(OnOptionsClicked);
        Quit.onClick.AddListener(OnQuitClicked);
        AcceptInputName.onClick.AddListener(SubmitName);
        BackFromOptionsButton.onClick.AddListener(BackToMainMenu);
        BackfromCreditsButton.onClick.AddListener(BackToMainMenu);

        if (CreditsButton != null)
            CreditsButton.onClick.AddListener(OnCreditsClicked);

        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        volumeSlider.value = 1.0f;

        SoundManager.Instance.PlayMusic(MusicTheme.MAIN_MENU);
        // Hide popups initially
        nameInputPopup.SetActive(false);
        optionsPopup.SetActive(false);
    }
    void SubmitName()
    {
        if(InputNameUI.text != null)
        {
            PlayerData.playerName = InputNameUI.text;
            SoundManager.Instance.StopCurrentMusic(2.0f);
            StartCoroutine(WaitnewScene("GAME", 5.0f));
        }
    }
    IEnumerator WaitnewScene(string scene,float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        SceneManager.LoadScene("GAME");
    }
    void OnStartGameClicked()
    {
        nameInputPopup.SetActive(true);
    }

    void OnOptionsClicked()
    {
        optionsPopup.SetActive(true);
    }

    void BackToMainMenu()
    {
        optionsPopup.SetActive(false);
        mainPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    void OnCreditsClicked()
    {
        optionsPopup.SetActive(false);
        creditsPanel.SetActive(true);
    }

    void OnVolumeChanged(float value)
    {
        //AudioListener.volume;
        soundManager.changeMaxVolume(value);
        soundManager.changeAllMusicVolume(value);
    }

    void OnQuitClicked()
    {
        Application.Quit();

        // If running in the editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
