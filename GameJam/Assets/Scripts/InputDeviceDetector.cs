using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceDetector : MonoBehaviour
{
    public string LastInputDevice = "KeyboardMouse";

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
    }
}