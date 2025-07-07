using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;



public class MainMenuUI : MonoBehaviour
{
    public Button StartGame;
    public Button Options;//volume credits
    public Button Quit;


    public GameObject nameInputPopup;
    public GameObject optionsPopup;
    public Button CreditsButton;
    public Slider volumeSlider;

    public TMP_InputField InputNameUI;
    public Button AcceptInputName;
    public string PlayerName;




    void Start()
    {
        StartGame.onClick.AddListener(OnStartGameClicked);
        Options.onClick.AddListener(OnOptionsClicked);
        Quit.onClick.AddListener(OnQuitClicked);
        AcceptInputName.onClick.AddListener(SubmitName);
        
        if (CreditsButton != null)
            CreditsButton.onClick.AddListener(OnCreditsClicked);

        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // Hide popups initially
        nameInputPopup.SetActive(false);
        optionsPopup.SetActive(false);
    }
    void SubmitName()
    {
        if(InputNameUI.text != null)
        {
            PlayerData.playerName = InputNameUI.text;
            SceneManager.LoadScene("GAME");
        }
    }
    void OnStartGameClicked()
    {
        nameInputPopup.SetActive(true);
    }

    void OnOptionsClicked()
    {
        optionsPopup.SetActive(true);
    }

    void OnCreditsClicked()
    {
        SceneManager.LoadScene("GAME_CREDITS"); // Replace with your actual credits scene name
    }

    void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
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
