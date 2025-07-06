using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class Textos : MonoBehaviour
{

    public GameObject exclamationImage;
    public GameObject uiBackground;
    public GameObject uiDialogue1;

    private float exclamationTimer = 0f;
    private float exclamationInterval = 5f;
    private bool dialogeActive = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiDialogue1.GetComponent<Text>();
        //canvasText.text = "Hola soy un texto";
        exclamationImage.SetActive(false);
        uiBackground.SetActive(false);
        uiDialogue1.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pulsada");
            PopUp();
        }
               
        if (uiDialogue1.activeSelf)
        {
            return;
        }
        
        exclamationTimer += Time.deltaTime;
        if (exclamationTimer >= exclamationInterval && !exclamationImage.activeSelf)
        {
            Debug.Log("Llegue a 5");
            exclamationImage.SetActive(true);
            exclamationTimer = 0f;
        }
    }
    void PopUp()
    {
        if (exclamationImage.activeSelf)
        {
            exclamationImage.SetActive(false);
            uiBackground.SetActive(true);
            uiDialogue1.SetActive(true);
            dialogeActive = true;

        }
        else if (dialogeActive)
        {
            uiBackground.SetActive(false);
            uiDialogue1.SetActive(false);
            dialogeActive = false;

            exclamationTimer = 0f;
        }
    }
}
