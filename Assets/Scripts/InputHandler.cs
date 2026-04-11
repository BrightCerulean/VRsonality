using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public QuestionPanel questionPanel;
    public string BButton;

    private void Awake()
    {
        if (Application.isEditor)//PC
        {
            BButton = "js2";
        }
        else//Android
        {
            BButton = "js1";
        }
    }

    void Update()
    {
        if (Input.GetButtonDown(BButton))
        {
            questionPanel.TogglePanel();
        }
    }
}