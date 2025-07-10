using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Textos : MonoBehaviour
{
    public static Textos Instance;
    public InputActionAsset inputActions;//todos los botones mapeados

    InputAction AcceptNotification;//una accion q se triggerea con un boton

    [SerializeField] private GameObject exclamationImage;
    public GameObject uiBackground;
    [SerializeField] private TextMeshProUGUI eventText;

    [NonSerialized] public bool ShowNotification;

    void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        exclamationImage.SetActive(false);
        uiBackground.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        exclamationImage.gameObject.SetActive(ShowNotification);
        float pressedInput = AcceptNotification.ReadValue<float>();

        // added this to skip 1 text per E press
        bool pressedThisFrame = AcceptNotification.WasPressedThisFrame();
        if (pressedInput == 1.0f && pressedThisFrame)
        {
            if (ShowNotification)
            {
                GameManager.Instance.LoadNewEvent();
            }
            else
            {
                EventsManager.Instance.PressedInput();
            }

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
        if (!uiBackground.activeInHierarchy) StartCoroutine(openTextBoxRoutine());
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
