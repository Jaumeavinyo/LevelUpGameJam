using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceDetector : MonoBehaviour
{
    public string LastInputDevice = "KeyboardMouse";
    public InputActionAsset inputActions;
    public InputAction escAction;

    public GameObject StopMenu;


    void Update()
    {
        // Detect Gamepad
        if (Gamepad.current != null)
        {
            var g = Gamepad.current;
            if (g.leftStick.ReadValue().magnitude > 0.1f ||
                g.rightStick.ReadValue().magnitude > 0.1f ||
                g.buttonSouth.wasPressedThisFrame ||
                g.dpad.ReadValue().magnitude > 0.1f)
            {
                LastInputDevice = "Gamepad";
                return;
            }
        }

        // Detect Keyboard or Mouse
        if (Keyboard.current.anyKey.wasPressedThisFrame ||
            Mouse.current.delta.ReadValue().magnitude > 0.1f ||
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            LastInputDevice = "KeyboardMouse";
        }
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            
            TogglePause();
        }

    }

    void TogglePause()
    {
        // Toggle between paused and unpaused
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        StopMenu.SetActive(true);
        // Optional: Pause audio
        //AudioListener.pause = Time.timeScale == 0;
    }

    void OnEnable()
    {
        InputActionMap inputActionsMap = inputActions.FindActionMap("Player", throwIfNotFound: true);

        escAction = inputActionsMap.FindAction("Esc", throwIfNotFound: true);
        escAction.Enable();
    }
    private void OnDisable()
    {
        escAction.Disable();
    }

}