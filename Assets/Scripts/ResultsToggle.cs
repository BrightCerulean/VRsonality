using UnityEngine;
using TMPro;

public class ResultsToggle : MonoBehaviour
{
    public TextMeshProUGUI personalityTitle;
    public TextMeshProUGUI personalityDescription;
    public TextMeshProUGUI toggleHint;
    public float resultFontSize = 10f;
    public float summaryFontSize = 8f;

    private bool showingSummary = false;
    public static string XButton;

    void Start()
    {
        ShowResults();
    }

    private void Awake()
    {
        if (Application.isEditor)//PC
        {
            XButton = "js1";
        }
        else//Android
        {
            XButton = "js2";
        }

    }
    void Update()
    {
        if (Input.GetButtonDown(XButton))
        {
            ToggleView();
        }
    }

    void ToggleView()
    {
        showingSummary = !showingSummary;

        if (showingSummary)
        {
            ShowSummary();
        }
        else
        {
            ShowResults();
        }
    }

    void ShowResults()
    {
        if (GameManager.Instance == null) return;

        personalityTitle.text =
            GameManager.Instance.GetResult();

        personalityDescription.text =
            GameManager.Instance.GetDescription();
        personalityDescription.fontSize = resultFontSize;

        if (toggleHint != null)
        {
            toggleHint.text =
                "Press X to view your journey";
        }
    }

    void ShowSummary()
    {
        if (GameManager.Instance == null) return;

        personalityTitle.text = "YOUR JOURNEY";

        personalityDescription.text =
            GameManager.Instance.GetChoiceSummary();
        personalityDescription.fontSize = summaryFontSize;

        if (toggleHint != null)
        {
            toggleHint.text =
                "Press X to return to results";
        }
    }
}