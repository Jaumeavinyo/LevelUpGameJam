using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Textos : MonoBehaviour
{

    public InputActionAsset inputActions;//todos los botones mapeados

    InputAction AcceptNotification;//una accion q se triggerea con un boton

    [SerializeField] private GameObject exclamationImage;
    public GameObject uiBackground;
    [SerializeField] private TextMeshProUGUI eventText;
    [NonSerialized] public float bcloseNotification;

    [NonSerialized] public bool ShowNotification;


    void Start()
    {
        exclamationImage.SetActive(false);
        uiBackground.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        exclamationImage.gameObject.SetActive(ShowNotification);
        bcloseNotification = AcceptNotification.ReadValue<float>();

        if (ShowNotification && bcloseNotification == 1.0f)
        {
            GameManager.Instance.LoadNewEvent();
        }
    }
    private IEnumerator openTextBoxRoutine()
    {
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        uiBackground.SetActive(true);
    }

    public void OpenEvent(string Dialogue)
    {
        eventText.text = Dialogue;
        ShowNotification = false;
        StartCoroutine(openTextBoxRoutine());
    }

    public void FinishEvent()
    {
        uiBackground.SetActive(false);
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
