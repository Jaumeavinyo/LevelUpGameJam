using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Textos : MonoBehaviour
{

    public InputActionAsset inputActions;//todos los botones mapeados

    InputAction AcceptNotification;//una accion q se triggerea con un boton

    public GameObject exclamationImage;
    public GameObject uiBackground;

  
    public GameObject DialogueToShowNext;

    public float bcloseNotification;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
        //canvasText.text = "Hola soy un texto";
        exclamationImage.SetActive(false);
        uiBackground.SetActive(false);    
    }

    // Update is called once per frame
    void Update()
    {     

        bcloseNotification = AcceptNotification.ReadValue<float>();

        if(bcloseNotification == 1.0f)
        {
            closeNotification();
        }
    }
   
    public void closeNotification()
    {
        exclamationImage.SetActive(false);
        uiBackground.SetActive(true);
        DialogueToShowNext.SetActive(true);   
    }

    public void CreateEventNotification(GameObject dialogueToOpen)
    {
        exclamationImage.SetActive(true);
        DialogueToShowNext = dialogueToOpen;
    }



    private void OnEnable()
    {
        //genero el mapa a partir de mi asset de inputs
        InputActionMap inputActionsMap = inputActions.FindActionMap("Player", throwIfNotFound: true);

        //setteo mi accion
        AcceptNotification = inputActionsMap.FindAction("Dash", throwIfNotFound: true);//dash tiene guardado input de E y input de el dash
        AcceptNotification.Enable();
        
    }
    private void OnDisable()
    {
        AcceptNotification.Disable();
    }
}
