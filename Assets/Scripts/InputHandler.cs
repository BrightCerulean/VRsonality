using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public QuestionPanel questionPanel;

    void Update()
    {
        if (Input.GetButtonDown("js2"))
        {
            questionPanel.TogglePanel();
        }
    }
}