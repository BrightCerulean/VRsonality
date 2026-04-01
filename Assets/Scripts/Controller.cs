using UnityEngine;

public class ControllerCheck : MonoBehaviour
{
    void Update()
    {
        string[] joysticks = Input.GetJoystickNames();
        if (joysticks.Length > 0)
        {
            foreach (string joy in joysticks)
            {
                Debug.Log("Controller found: " + joy);
            }
        }
        else
        {
            Debug.Log("No controller detected!");
        }
    }
}

//figure out a way to use keyboard arrow keys for movement