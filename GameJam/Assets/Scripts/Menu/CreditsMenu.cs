using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
public class CreditsMenu : MonoBehaviour
{
    public Button backButton;

    
    void Start()
    {
        backButton.onClick.AddListener(backClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void backClicked()
    {
        SceneManager.LoadScene("GAME_MENU");
    }
}
