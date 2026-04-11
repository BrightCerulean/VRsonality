using UnityEngine;
using TMPro;

public class ResultsDisplay : MonoBehaviour
{
    public TextMeshProUGUI personalityTitle;
    public TextMeshProUGUI personalityDescription;
    public TextMeshProUGUI selectionsLog;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[Results] GameManager not found");
            return;
        }

        personalityTitle.text = 
            GameManager.Instance.GetResult();
        personalityDescription.text = 
            GameManager.Instance.GetDescription();
        selectionsLog.text = "Your choices: " + 
            GameManager.Instance.GetSelectionsString();
    }
}