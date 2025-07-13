using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StopMenu : MonoBehaviour
{

    public Button Resume;
    public Button Quit;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Quit.onClick.AddListener(OnQuitClicked);
        Resume.onClick.AddListener(OnResumeClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnResumeClicked()
    {
        // Unpause the game
        Time.timeScale = 1f;

        // Hide this pause menu
        gameObject.SetActive(false);

       
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
