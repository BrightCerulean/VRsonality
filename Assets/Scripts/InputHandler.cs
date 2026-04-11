using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public QuestionPanel questionPanel;
    public static string BButton;

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

        //Debug button
        if (Input.GetButtonDown("js9"))
        {
            Debug.Log(GameManager.Instance.GetSelectionsString());
        }
    }
}