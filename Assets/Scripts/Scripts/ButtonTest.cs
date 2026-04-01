using UnityEngine;
using TMPro;

public class ButtonTest : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    private string lastPressed = "Press any button...";

    void Update()
    {
        // Check all joystick buttons
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown((KeyCode)(360 + i)))
            {
                lastPressed = "JoystickButton" + i;
                Debug.Log(lastPressed);
            }
        }

        // Check named inputs
        string[] jsNames = {"js0","js1","js2","js3","js4","js5","js6","js7","js8","js9","js10","js11","js12","js13"};
        foreach (string js in jsNames)
        {
            if (Input.GetButtonDown(js))
            {
                lastPressed = js + " pressed";
                Debug.Log(lastPressed);
            }
        }

        if (displayText != null)
            displayText.text = lastPressed;
    }
}